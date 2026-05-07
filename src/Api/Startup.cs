namespace Azoxia.Core.Api
{
    using Azoxia.Api.Identity;
    using Azoxia.Core.Api.DependencyInjection;
    using Azoxia.Core.Api.Middleware;
    using Azoxia.Core.Wrappers;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Identity;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.OpenApi;

    /// <summary>
    /// Bootstraps the ASP.NET Core host and exposes extension points (<see cref="OnConfigureServices"/>, <see cref="OnConfigurePipelines"/>)
    /// so callers can register services and configure the HTTP pipeline without replacing the default lifecycle.
    /// </summary>
    public class Startup
    {
        #region Fields

        /// <summary>
        /// CORS policy name registered for browser clients (SPA). Origins come from configuration key <c>Cors:AllowedOrigins</c>.
        /// </summary>
        private const string CorsPolicyName = "Frontend";

        #endregion Fields

        #region Events

        /// <summary>
        /// Invoked after the baseline HTTP pipeline runs (exceptions, HTTPS, compression, authorization) and core endpoints are mapped.
        /// Subscribe to add middleware, routes, authentication, or other pipeline concerns (<see cref="WebApplication"/> extension pattern).
        /// </summary>
        public event Action<WebApplication>? OnConfigurePipelines;

        /// <summary>
        /// Invoked while building <see cref="WebApplication"/> (after <see cref="WebApplication.CreateBuilder(string[])"/>),
        /// before <see cref="WebApplicationBuilder.Build"/>. Subscribe to register services, options, and other DI configuration.
        /// </summary>
        public event Action<WebApplicationBuilder>? OnConfigureServices;

        #endregion Events

        #region Utils

        /// <summary>
        /// Configures middleware, HTTPS, CORS, compression, authorization, and default endpoints; invokes <see cref="OnConfigurePipelines"/>.
        /// </summary>
        /// <param name="app">The built web application.</param>
        private void ConfigurePipelines(WebApplication app)
        {
            bool isDevelopment = app.Environment.IsDevelopment();

            // JSON <see cref="ApiResponse"/> for all unhandled exceptions (same shape in every environment).
            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = ApiResponseExceptionHandler.WriteAsync,
            });

            // Instruct browsers to enforce HTTPS on future visits (skipped in Development to simplify local setups).
            if (!isDevelopment)
            {
                app.UseHsts();
            }

            // Prefer TLS only when an HTTPS port is explicitly configured.
            string? httpsPort = app.Configuration["ASPNETCORE_HTTPS_PORT"];
            if (!string.IsNullOrWhiteSpace(httpsPort))
            {
                app.UseHttpsRedirection();
            }

            // Must run early so preflight (OPTIONS) and response headers honor the SPA origin before auth/compression finalize the response.
            app.UseCors(CorsPolicyName);

            // Compress responses when Accept-Encoding permits (gzip/Brotli depends on DI configuration).
            app.UseResponseCompression();

            // Must run before UseAuthorization when JWT/cookie schemes are registered (no-op if no authentication middleware is configured).
            app.UseAuthentication();

            // Evaluates authorization policies and [Authorize] metadata before MVC/minimal endpoints run.
            app.UseAuthorization();

            // Enforces endpoint-scoped RBAC permissions for authenticated callers.
            app.UseMiddleware<PermissionEnforcementMiddleware>();

            // Discovery document for codegen, gateways, or contract tooling (Swagger UI remains opt-in separately).
            if (isDevelopment)
            {
                app.MapOpenApi();
            }

            // K8s/orchestration-friendly liveness and readiness probing (matches AddHealthChecks()).
            app.MapHealthChecks("/health");

            // Attribute-routed MVC API controllers registered in DI.
            app.MapControllers();

            OnConfigurePipelines?.Invoke(app);
        }

        /// <summary>
        /// Registers controllers, OpenAPI, health checks, Problem Details, compression, CORS, and other services; invokes <see cref="OnConfigureServices"/>.
        /// </summary>
        /// <param name="builder">The web application builder.</param>
        private void ConfigureServices(WebApplicationBuilder builder)
        {
            // Shared primitives: register HttpContext as a request-scoped service for dependents.
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IExecutionContext, HttpExecutionContext>();

            // In-memory caching used by middleware, controllers, filters, and background collaborators.
            builder.Services.AddMemoryCache();

            // API surface: MVC controllers, model binding, and action filters — invalid model state returns <see cref="ApiResponse"/>.
            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = static context =>
                    {
                        List<string> fieldErrors = [];
                        foreach (KeyValuePair<string, Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateEntry> pair in context.ModelState)
                        {
                            if (pair.Value.Errors.Count == 0)
                            {
                                continue;
                            }

                            foreach (Microsoft.AspNetCore.Mvc.ModelBinding.ModelError err in pair.Value.Errors)
                            {
                                string field = string.IsNullOrEmpty(pair.Key) ? "_" : pair.Key;
                                string msg = string.IsNullOrEmpty(err.ErrorMessage)
                                    ? "The value is invalid."
                                    : err.ErrorMessage;
                                fieldErrors.Add($"{field}: {msg}");
                            }
                        }

                        ApiResponse body = ApiResponse.Failure(
                            message: AzoxiaErrorCodes.RequestValidationFailed.ErrorMessage,
                            errorCode: AzoxiaErrorCodes.RequestValidationFailed.Code,
                            errors: fieldErrors);

                        return new BadRequestObjectResult(body);
                    };
                });

            // OpenAPI 3 document services (machine-readable schema for tooling, gateways, codegen). Does not enable Swagger UI.
            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, _, _) =>
                {
                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

                    document.Components.SecuritySchemes[JwtBearerDefaults.AuthenticationScheme] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Name = "Authorization",
                        Description = "JWT Bearer authentication. Example: Bearer {token}",
                    };

                    return Task.CompletedTask;
                });
            });

            // Authorization policies, [Authorize], and IAuthorizationService registration.
            builder.Services.AddAuthorization();

            // JWT Bearer validation (symmetric key from <c>JwtConfig</c> — see <see cref="JwtAuthenticationServiceCollectionExtensions"/>).
            builder.Services.AddAzoxiaJwtBearerAuthentication(builder.Configuration);

            // Liveness/readiness probes and dependency monitoring via configured health checks.
            builder.Services.AddHealthChecks();

            // RFC 7807 Problem Details for consistent, structured JSON error payloads.
            builder.Services.AddProblemDetails();

            // Response compression middleware services (gzip/Brotli negotiation and encoding).
            builder.Services.AddResponseCompression();

            // Browser cross-origin requests. Origins listed under Cors:AllowedOrigins with dev-friendly defaults.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyName, policy =>
                {
                    policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
                });
            });

            OnConfigureServices?.Invoke(builder);
        }

        #endregion Utils

        #region Methods

        /// <summary>
        /// Builds the web host from command-line arguments, runs service and pipeline configuration, then starts the server (blocking).
        /// </summary>
        /// <param name="args">Command-line arguments passed to the hosting process (typically from <c>Environment.GetCommandLineArgs()</c> or <c>Main</c>).</param>
        public void Run(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder);
            WebApplication app = builder.Build();
            ConfigurePipelines(app);

            app.Run();
        }

        /// <summary>
        /// Builds the web host from command-line arguments, runs service and pipeline configuration, then starts the server asynchronously.
        /// </summary>
        /// <param name="args">Command-line arguments passed to the hosting process (typically from <c>Environment.GetCommandLineArgs()</c> or <c>Main</c>).</param>
        /// <returns>A <see cref="Task"/> that completes when the application stops.</returns>
        public Task RunAsync(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder);
            WebApplication app = builder.Build();
            ConfigurePipelines(app);

            return app.RunAsync();
        }

        #endregion Methods
    }
}
