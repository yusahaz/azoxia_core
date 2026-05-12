namespace Azoxia.Core.Api.Middleware
{
    using System;
    using System.Net;
    using System.Text.Json;
    using Azoxia.Core.Wrappers;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Writes <see cref="ApiResponse"/> JSON for the global exception handler pipeline.
    /// </summary>
    public static class ApiResponseExceptionHandler
    {
        #region Fields

        private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

        #endregion Fields

        #region Methods

        /// <summary>
        /// Exception handler delegate suitable for <see cref="ExceptionHandlerOptions.ExceptionHandler"/>.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <returns>A task that completes when the response has been written.</returns>
        public static async Task WriteAsync(HttpContext context)
        {
            IExceptionHandlerFeature? feature = context.Features.Get<IExceptionHandlerFeature>();
            Exception? error = feature?.Error;

            ILogger? logger = context.RequestServices
                .GetService<ILoggerFactory>()
                ?.CreateLogger("ApiResponseExceptionHandler");

            if (error is null)
            {
                logger?.LogError("Unhandled exception pipeline executed without an exception payload.");
                Console.Error.WriteLine("Unhandled exception pipeline executed without an exception payload.");
                await WriteBodyAsync(
                        context,
                        StatusCodes.Status500InternalServerError,
                        ApiResponse.Failure(
                            statusCode: HttpStatusCode.InternalServerError,
                            message: "An unexpected error occurred. Please try again later.",
                            errorCode: "AZX_CORE_INTERNAL"))
                    .ConfigureAwait(false);
                return;
            }

            logger?.LogError(error, "Unhandled exception caught by global API exception handler.");
            if (logger is null)
            {
                Console.Error.WriteLine("Unhandled exception caught by global API exception handler (no ILogger available).");
                Console.Error.WriteLine(error.ToString());
            }

            IWebHostEnvironment? environment = context.RequestServices.GetService<IWebHostEnvironment>();
            bool includeInternalDetails = environment?.IsDevelopment() == true;
            (int statusCode, ApiResponse body) = MapException(error, includeInternalDetails);

            await WriteBodyAsync(context, statusCode, body).ConfigureAwait(false);
        }

        private static (int StatusCode, ApiResponse Body) MapException(Exception error, bool includeInternalDetails)
        {
            if (error is RequestValidationException requestValidationException)
            {
                IReadOnlyList<string> fields = requestValidationException.Failures
                    .Select(f => $"{f.Field}: {f.Message}")
                    .ToList();

                return (
                    StatusCodes.Status400BadRequest,
                    ApiResponse.Failure(
                        message: AzoxiaErrorCodes.RequestValidationFailed.ErrorMessage,
                        errorCode: AzoxiaErrorCodes.RequestValidationFailed.Code,
                        errors: fields));
            }

            if (error is AzoxiaException azoxiaException)
            {
                int status = string.Equals(azoxiaException.Error.Code, AzoxiaErrorCodes.NotFound.Code, StringComparison.Ordinal)
                    ? StatusCodes.Status404NotFound
                    : string.Equals(azoxiaException.Error.Code, AzoxiaErrorCodes.PermissionDenied.Code, StringComparison.Ordinal)
                        ? StatusCodes.Status403Forbidden
                        : StatusCodes.Status400BadRequest;

                return (
                    status,
                    ApiResponse.Failure(
                        message: azoxiaException.Error.ErrorMessage,
                        errorCode: azoxiaException.Error.Code));
            }

            return (
                StatusCodes.Status500InternalServerError,
                ApiResponse.Failure(
                    statusCode: HttpStatusCode.InternalServerError,
                    message: "An unexpected error occurred. Please try again later.",
                    errorCode: "AZX_CORE_INTERNAL",
                    errors: includeInternalDetails ? [error.ToString()] : null));
        }

        private static async Task WriteBodyAsync(HttpContext context, int statusCode, ApiResponse body)
        {
            if (!context.Response.HasStarted)
            {
                context.Response.Clear();
            }

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json; charset=utf-8";

            await context.Response.WriteAsJsonAsync(body, SerializerOptions, context.RequestAborted)
                .ConfigureAwait(false);
        }

        #endregion Methods
    }
}
