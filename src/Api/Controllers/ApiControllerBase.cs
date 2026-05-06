namespace Azoxia.Core.Api.Controllers
{
    using Azoxia.Core.Wrappers;
    using Azoxia.Core.Application;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Abstract base for API controllers: enables <see cref="ApiControllerAttribute"/> conventions (automatic model validation, binding inference, Problem Details behavior),
    /// requires an authenticated caller via <see cref="AuthorizeAttribute"/>, and applies a consistent <c>[controller]/[action]</c> route template.
    /// </summary>
    /// <param name="serviceProvider">Supplies cross-cutting services (for example logging) to the controller instance.</param>
    [ApiController]
    [Authorize]
    [Route("[controller]/[action]")]
    public abstract class ApiControllerBase(IServiceProvider serviceProvider) :
        ControllerBase
    {
        #region Fields

        /// <summary>
        /// Lazily populated by <see cref="Logger"/> on first access; avoids resolving <see cref="ILogger"/> until logging is needed.
        /// </summary>
        private ILogger? _logger;

        #endregion Fields

        #region Utils

        /// <summary>
        /// Dispatches a <see cref="ICommand"/> (no typed payload) and returns <c>200 OK</c> with an empty body after successful handling.
        /// </summary>
        /// <param name="command">The command instance to execute.</param>
        /// <param name="cancellationToken">Propagates cancellation from the HTTP request.</param>
        /// <returns><c>OK</c> with an <see cref="ApiResponse"/> envelope when the pipeline completes without fault.</returns>
        protected async Task<IActionResult> ExecuteCommand(
            ICommand command,
            CancellationToken cancellationToken = default)
        {
            await ServiceProvider
                .GetRequiredService<ISender>()
                .SendAsync(command, cancellationToken);

            return Ok(ApiResponse.Success());
        }

        /// <summary>
        /// Dispatches a typed command and returns <c>200 OK</c> with the handler result body.
        /// </summary>
        /// <typeparam name="TResult">The command result type produced by the handler.</typeparam>
        /// <param name="command">The command instance to execute.</param>
        /// <param name="cancellationToken">Propagates cancellation from the HTTP request.</param>
        /// <returns><c>OK</c> with <see cref="ApiResponse{T}"/> wrapping <typeparamref name="TResult"/>.</returns>
        protected async Task<IActionResult> ExecuteCommand<TResult>(
            ICommand<TResult> command,
            CancellationToken cancellationToken = default)
        {
            TResult result = await ServiceProvider
                .GetRequiredService<ISender>()
                .SendAsync(command, cancellationToken);

            return Ok(ApiResponse<TResult>.Success(result));
        }

        /// <summary>
        /// Executes <paramref name="query"/> through the application query pipeline (read-only path) and returns an <see cref="IActionResult"/> representing <typeparamref name="TResult"/> or a failure outcome.
        /// </summary>
        /// <typeparam name="TResult">The read model or DTO type returned by the query handler.</typeparam>
        /// <param name="query">The query object describing what to load (projection, paging, filtering, etc. according to downstream rules).</param>
        /// <param name="cancellationToken">Propagates cancellation from the HTTP request.</param>
        /// <returns><c>OK</c> with <see cref="ApiResponse{T}"/> wrapping the query result.</returns>
        protected async Task<IActionResult> ExecuteQuery<TResult>(
            IQuery<TResult> query,
            CancellationToken cancellationToken = default)
        {
            TResult result = await ServiceProvider
                .GetRequiredService<ISender>()
                .SendAsync(query, cancellationToken);

            return Ok(ApiResponse<TResult>.Success(result));
        }

        /// <summary>
        /// Executes paged query and returns <see cref="PageableApiResponse{T}"/> envelope.
        /// </summary>
        /// <typeparam name="T">Row model type.</typeparam>
        /// <param name="query">Paged query object.</param>
        /// <param name="itemsSelector">Maps query result into row list.</param>
        /// <param name="totalCountSelector">Maps query result into total count.</param>
        /// <param name="limitSelector">Maps query result into applied limit.</param>
        /// <param name="offsetSelector">Maps query result into applied offset.</param>
        /// <param name="cancellationToken">Propagates cancellation from the HTTP request.</param>
        /// <typeparam name="TResult">Paged query result model type.</typeparam>
        /// <returns><c>OK</c> with <see cref="PageableApiResponse{T}"/> wrapping list + page metadata.</returns>
        protected async Task<IActionResult> ExecutePageQuery<T, TResult>(
            IQuery<TResult> query,
            Func<TResult, IReadOnlyList<T>> itemsSelector,
            Func<TResult, int> totalCountSelector,
            Func<TResult, int> limitSelector,
            Func<TResult, int> offsetSelector,
            CancellationToken cancellationToken = default)
        {
            TResult result = await ServiceProvider
                .GetRequiredService<ISender>()
                .SendAsync(query, cancellationToken);

            return Ok(PageableApiResponse<T>.Success(
                data: itemsSelector(result),
                totalCount: totalCountSelector(result),
                limit: limitSelector(result),
                offset: offsetSelector(result)));
        }

        #endregion Utils

        #region Properties

        /// <summary>
        /// Category logger named after the concrete controller type, resolved once from <see cref="ILoggerFactory"/> on first use.
        /// </summary>
        protected ILogger Logger
            => _logger ??= ServiceProvider
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger(GetType());

        /// <summary>
        /// Root service provider captured from the primary constructor; used to resolve infrastructure services (for example logging factories) for derived controllers.
        /// </summary>
        protected IServiceProvider ServiceProvider => serviceProvider;

        #endregion Properties
    }
}
