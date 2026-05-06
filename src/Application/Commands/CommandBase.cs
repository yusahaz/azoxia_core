namespace Azoxia.Core.Application.Commands
{
    /// <summary>
    /// Marker base for commands that do not return a typed payload (see <see cref="ICommand"/> / <see cref="Unit"/>).
    /// </summary>
    public abstract class CommandBase :
        ICommand
    {
    }

    /// <summary>
    /// Marker base for commands that return <typeparamref name="TResult"/> when handled.
    /// </summary>
    /// <typeparam name="TResult">The result type produced by the command handler.</typeparam>
    public abstract class CommandBase<TResult> :
        ICommand<TResult>
    {
    }
}
