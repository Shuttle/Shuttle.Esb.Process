using System;

namespace Shuttle.Esb.Process
{
    public class ProcessException : Exception
    {
        public ProcessException(string message) : base(message)
        {
        }
    }
}