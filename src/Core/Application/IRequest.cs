namespace Azoxia.Core.Application
{
    /// <summary>
    /// Base contract for an application request that produces <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of value returned when the request is handled.</typeparam>
    public interface IRequest<TResult>
    {
    }
}
