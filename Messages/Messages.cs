using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages
{
    public interface IOperationARequested : IOperationRequestedBase
    {
    }

    public interface IOperationBRequested : IOperationRequestedBase
    {
        string Username { get; }
    }

    public interface IOperationRequestedBase
    {
    }

    public class OperationARequestedEvent : IOperationARequested
    {
    }

    public class OperationBRequestedEvent : IOperationBRequested
    {
        public string Username { get; set; }
    }
}