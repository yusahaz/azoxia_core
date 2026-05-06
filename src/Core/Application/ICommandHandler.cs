namespace Azoxia.Core.Application
{
    /// <summary>
    /// Handles commands of type <typeparamref name="TCommand"/> that complete without a typed payload.
    /// </summary>
    /// <typeparam name="TCommand">The command type.</typeparam>
    public interface ICommandHandler<in TCommand> :
        IRequestHandler<TCommand, Unit>
        where TCommand : ICommand
    {
    }

    /// <summary>
    /// Handles commands of type <typeparamref name="TCommand"/> and returns <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TCommand">The command type.</typeparam>
    /// <typeparam name="TResult">The handler result type.</typeparam>
    public interface ICommandHandler<in TCommand, TResult> :
        IRequestHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
    }
}
