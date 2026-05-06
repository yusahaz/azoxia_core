namespace Azoxia.Core.Wrappers
{
    using System.Net;

    /// <summary>
    /// Represents a standardized API response envelope.
    /// </summary>
    /// <remarks>
    /// Use this type to return consistent success/failure metadata from application or API layers.
    /// For payload-carrying results, use <see cref="ApiResponse{T}"/>.
    /// </remarks>
    public record ApiResponse
    {
        #region Methods

        /// <summary>
        /// Creates a failed response.
        /// </summary>
        /// <param name="statusCode">Status code for the response (defaults to <see cref="HttpStatusCode.BadRequest"/>).</param>
        /// <param name="errorCode">Application-level error code.</param>
        /// <param name="message">Human-readable summary.</param>
        /// <param name="errors">Individual field-level error messages for validation failures.</param>
        /// <returns>The failure envelope.</returns>
        public static ApiResponse Failure(
            HttpStatusCode statusCode = HttpStatusCode.BadRequest,
            string? errorCode = null,
            string? message = null,
            IReadOnlyList<string>? errors = null) =>
            new ApiResponse
            {
                IsSuccess = false,
                StatusCode = statusCode,
                ErrorCode = errorCode,
                Message = message,
                Errors = errors
            };

        /// <summary>
        /// Creates a successful response.
        /// </summary>
        /// <param name="statusCode">Status code for the response (defaults to <see cref="HttpStatusCode.OK"/>).</param>
        /// <param name="message">Optional message.</param>
        /// <returns>The success envelope.</returns>
        public static ApiResponse Success(HttpStatusCode statusCode = HttpStatusCode.OK, string? message = null) =>
            new ApiResponse
            {
                IsSuccess = true,
                StatusCode = statusCode,
                Message = message
            };

        #endregion Methods

        #region Properties

        /// <summary>
        /// Gets the application-level error code identifying the failure reason.
        /// </summary>
        public string? ErrorCode { get; init; }

        /// <summary>
        /// Gets individual validation error messages. Populated only for 400 validation failures.
        /// </summary>
        public IReadOnlyList<string>? Errors { get; init; }

        /// <summary>
        /// Gets a value indicating whether the operation completed successfully.
        /// </summary>
        public bool IsSuccess { get; init; }

        /// <summary>
        /// Gets an optional human-readable message (e.g., validation summary or error detail).
        /// </summary>
        public string? Message { get; init; }

        /// <summary>
        /// Gets the HTTP status code that best describes the outcome.
        /// </summary>
        public HttpStatusCode StatusCode { get; init; }

        #endregion Properties
    }

    /// <summary>
    /// Represents a standardized API response envelope with a payload.
    /// </summary>
    /// <typeparam name="T">The payload type.</typeparam>
    /// <remarks>
    /// Prefer this type for query results or commands that return data.
    /// If you only need metadata (no payload), use <see cref="ApiResponse"/>.
    /// </remarks>
    public record ApiResponse<T> : ApiResponse
    {
        #region Methods

        /// <summary>
        /// Creates a failed response (no payload).
        /// </summary>
        /// <param name="statusCode">Status code for the response (defaults to <see cref="HttpStatusCode.BadRequest"/>).</param>
        /// <param name="errorCode">Application-level error code.</param>
        /// <param name="message">Human-readable summary.</param>
        /// <param name="errors">Individual field-level error messages for validation failures.</param>
        /// <returns>The failure envelope.</returns>
        public static new ApiResponse<T> Failure(
            HttpStatusCode statusCode = HttpStatusCode.BadRequest,
            string? errorCode = null,
            string? message = null,
            IReadOnlyList<string>? errors = null) =>
            new ApiResponse<T>
            {
                IsSuccess = false,
                StatusCode = statusCode,
                ErrorCode = errorCode,
                Message = message,
                Errors = errors,
                Data = default
            };

        /// <summary>
        /// Creates a successful response with a payload.
        /// </summary>
        /// <param name="data">The payload.</param>
        /// <param name="statusCode">Status code for the response (defaults to <see cref="HttpStatusCode.OK"/>).</param>
        /// <param name="message">Optional message.</param>
        /// <returns>The success envelope.</returns>
        public static ApiResponse<T> Success(
            T data,
            HttpStatusCode statusCode = HttpStatusCode.OK,
            string? message = null) =>
            new ApiResponse<T>
            {
                IsSuccess = true,
                StatusCode = statusCode,
                Message = message,
                Data = data
            };

        #endregion Methods

        #region Properties

        /// <summary>
        /// Gets the response payload.
        /// </summary>
        public T? Data { get; init; }

        #endregion Properties
    }

    /// <summary>
    /// Represents a standardized paged API response envelope with list payload and page metadata.
    /// </summary>
    /// <typeparam name="T">The row model type.</typeparam>
    /// <remarks>
    /// Prefer this type for list endpoints returning offset/limit based windows.
    /// </remarks>
    public record PageableApiResponse<T> : ApiResponse
    {
        #region Methods

        /// <summary>
        /// Creates a failed paged response (no list payload).
        /// </summary>
        /// <param name="statusCode">Status code for the response (defaults to <see cref="HttpStatusCode.BadRequest"/>).</param>
        /// <param name="errorCode">Application-level error code.</param>
        /// <param name="message">Human-readable summary.</param>
        /// <param name="errors">Individual field-level error messages for validation failures.</param>
        /// <returns>The failed paged envelope.</returns>
        public static new PageableApiResponse<T> Failure(
            HttpStatusCode statusCode = HttpStatusCode.BadRequest,
            string? errorCode = null,
            string? message = null,
            IReadOnlyList<string>? errors = null) =>
            new PageableApiResponse<T>
            {
                IsSuccess = false,
                StatusCode = statusCode,
                ErrorCode = errorCode,
                Message = message,
                Errors = errors,
                Data = null,
                TotalCount = 0,
                Limit = 0,
                Offset = 0,
                HasMore = false
            };

        /// <summary>
        /// Creates a successful paged response with payload and paging metadata.
        /// </summary>
        /// <param name="data">The current page rows.</param>
        /// <param name="totalCount">Total row count before paging.</param>
        /// <param name="limit">Requested page size.</param>
        /// <param name="offset">Requested zero-based row offset.</param>
        /// <param name="statusCode">Status code for the response (defaults to <see cref="HttpStatusCode.OK"/>).</param>
        /// <param name="message">Optional message.</param>
        /// <returns>The success paged envelope.</returns>
        public static PageableApiResponse<T> Success(
            IReadOnlyList<T> data,
            int totalCount,
            int limit,
            int offset,
            HttpStatusCode statusCode = HttpStatusCode.OK,
            string? message = null) =>
            new PageableApiResponse<T>
            {
                IsSuccess = true,
                StatusCode = statusCode,
                Message = message,
                Data = data,
                TotalCount = totalCount,
                Limit = limit,
                Offset = offset,
                HasMore = offset + data.Count < totalCount
            };

        #endregion Methods

        #region Properties

        /// <summary>
        /// Gets the current page rows.
        /// </summary>
        public IReadOnlyList<T>? Data { get; init; }

        /// <summary>
        /// Gets a value indicating whether there are more rows after this page.
        /// </summary>
        public bool HasMore { get; init; }

        /// <summary>
        /// Gets requested page size.
        /// </summary>
        public int Limit { get; init; }

        /// <summary>
        /// Gets requested zero-based row offset.
        /// </summary>
        public int Offset { get; init; }

        /// <summary>
        /// Gets total row count before paging.
        /// </summary>
        public int TotalCount { get; init; }

        #endregion Properties
    }
}
