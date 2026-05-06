namespace Azoxia.Core.Application
{
    /// <summary>
    /// Represents the final handler invocation (or the next pipeline stage) as a parameterless async delegate.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    public delegate Task<TResult> RequestHandlerDelegate<TResult>();
}
