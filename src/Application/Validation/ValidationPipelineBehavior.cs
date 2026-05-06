namespace Azoxia.Core.Application.Validation
{
    using Azoxia.Core.Extensions;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Runs all registered request validators before invoking next pipeline stage.
    /// </summary>
    public sealed class ValidationPipelineBehavior<TRequest, TResult>(IServiceProvider serviceProvider) :
        IPipelineBehavior<TRequest, TResult>
        where TRequest : class, IRequest<TResult>
    {
        #region Methods

        public Task<TResult> HandleAsync(
            TRequest request,
            RequestHandlerDelegate<TResult> next,
            CancellationToken cancellationToken)
        {
            request.ThrowIfNull();
            next.ThrowIfNull();

            IReadOnlyList<ValidationFailure> failures = serviceProvider
                .GetServices<IRequestValidator<TRequest>>()
                .Select(v => v.Validate(request))
                .Where(result => !result.IsValid)
                .SelectMany(result => result.Errors)
                .ToList();

            if (failures.Count > 0)
            {
                throw new RequestValidationException(failures);
            }

            return next();
        }

        #endregion Methods
    }
}
