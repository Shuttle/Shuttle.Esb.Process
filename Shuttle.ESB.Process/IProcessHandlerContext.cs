using Shuttle.ESB.Core;
using Shuttle.Recall.Core;

namespace Shuttle.ESB.Process
{
    public interface IProcessHandlerContext<out T> : IHandlerContext<T> where T : class
    {
        IKeyStore KeyStore { get; }
        EventStream Stream { get; }
    }
}