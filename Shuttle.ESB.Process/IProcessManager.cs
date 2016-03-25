using System;

namespace Shuttle.Esb.Process
{
    public interface IProcessManager
    {
        Guid CorrelationId { get; set; } 
    }
}