namespace Azoxia.Core.Application
{
    /// <summary>
    /// Marker for a command that completes without a typed result (maps to <see cref="Unit"/>).
    /// </summary>
    public interface ICommand :
        IRequest<Unit>
    {
    }

    /// <summary>
    /// Marker for a command that yields <typeparamref name="TResult"/> when handled.
    /// </summary>
    /// <typeparam name="TResult">The result type returned by the handler.</typeparam>
    public interface ICommand<TResult> :
        IRequest<TResult>
    {
    }
}
