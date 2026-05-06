namespace Azoxia.Core.Api.Middleware
{
    using System.Text.Json;
    using Azoxia.Core.Wrappers;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Hosting;

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

            IWebHostEnvironment? env = context.RequestServices.GetService<IWebHostEnvironment>();
            bool isDevelopment = env?.IsDevelopment() == true;

            if (error is null)
            {
                await WriteBodyAsync(
                        context,
                        StatusCodes.Status500InternalServerError,
                        ApiResponse.Failure(message: "An unexpected error occurred.", errorCode: "AZX_CORE_INTERNAL"))
                    .ConfigureAwait(false);
                return;
            }

            (int statusCode, ApiResponse body) = MapException(error, isDevelopment);

            await WriteBodyAsync(context, statusCode, body).ConfigureAwait(false);
        }

        private static (int StatusCode, ApiResponse Body) MapException(Exception error, bool isDevelopment)
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
                    : StatusCodes.Status400BadRequest;

                return (
                    status,
                    ApiResponse.Failure(
                        message: azoxiaException.Error.ErrorMessage,
                        errorCode: azoxiaException.Error.Code));
            }

            string message = isDevelopment && error.Message.Length > 0
                ? error.Message
                : "An unexpected error occurred.";

            return (
                StatusCodes.Status500InternalServerError,
                ApiResponse.Failure(message: message, errorCode: "AZX_CORE_INTERNAL"));
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
