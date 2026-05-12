namespace Azoxia.Core.Api.Middleware
{
    using Azoxia.Core.Api.Authorization;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Identity;
    using Azoxia.Core.Extensions;
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Enforces <see cref="RequiresPermissionAttribute"/> when the host application registers a permission resolver type;
    /// otherwise the pipeline continues (role-based authorization only).
    /// </summary>
    public sealed class PermissionEnforcementMiddleware(RequestDelegate next)
    {
        private const string SystemUserIdClaim = "system_user_id";
        private const string EmployerIdClaim = "employer_id";

        private readonly RequestDelegate _next = next;

        /// <summary>
        /// Executes request permission evaluation.
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            Microsoft.AspNetCore.Http.Endpoint? endpoint = context.GetEndpoint();
            if (endpoint is null)
            {
                await _next(context).ConfigureAwait(false);
                return;
            }

            RequiresPermissionAttribute? permissionAttr =
                endpoint.Metadata.GetMetadata<RequiresPermissionAttribute>();

            if (permissionAttr is null)
            {
                await _next(context).ConfigureAwait(false);
                return;
            }

            IExecutionContext executionContext = context.RequestServices.GetRequiredService<IExecutionContext>();
            if (!executionContext.IsAuthenticated)
            {
                throw new AzoxiaException(AzoxiaErrorCodes.PermissionDenied);
            }

            string? systemUserIdRaw = executionContext.GetClaim(SystemUserIdClaim);
            systemUserIdRaw.ThrowIfNullOrWhiteSpace(AzoxiaErrorCodes.SystemUserIdClaimRequired);

            bool parsed = int.TryParse(systemUserIdRaw, out int systemUserId) && systemUserId > 0;
            parsed.ThrowIfFalse(AzoxiaErrorCodes.SystemUserIdClaimRequired);

            int? employerId = null;
            string? employerIdRaw = executionContext.GetClaim(EmployerIdClaim);
            if (!string.IsNullOrWhiteSpace(employerIdRaw) &&
                int.TryParse(employerIdRaw, out int parsedEmployerId) &&
                parsedEmployerId > 0)
            {
                employerId = parsedEmployerId;
            }

            Type? resolverType = Type.GetType(
                "Azoxia.AdaIsAkademi.Application.Services.IPermissionResolver, Azoxia.AdaIsAkademi.Application",
                throwOnError: false);

            if (resolverType is null)
            {
                // Application may not register a permission resolver (role-based auth only).
                await _next(context).ConfigureAwait(false);
                return;
            }

            object? resolver = context.RequestServices.GetService(resolverType);
            if (resolver is null)
            {
                throw new AzoxiaException(AzoxiaErrorCodes.PermissionDenied);
            }

            MethodInfo? hasPermissionMethod = resolverType.GetMethod("HasPermissionAsync");
            if (hasPermissionMethod is null)
            {
                throw new AzoxiaException(AzoxiaErrorCodes.PermissionDenied);
            }

            object? result = hasPermissionMethod.Invoke(
                obj: resolver,
                parameters: new object?[]
                {
                    systemUserId,
                    employerId,
                    permissionAttr.Permission,
                    context.RequestAborted
                });

            if (result is not Task<bool> task)
            {
                throw new AzoxiaException(AzoxiaErrorCodes.PermissionDenied);
            }

            bool allowed = await task.ConfigureAwait(false);

            allowed.ThrowIfFalse(AzoxiaErrorCodes.PermissionDenied);

            await _next(context).ConfigureAwait(false);
        }
    }
}

