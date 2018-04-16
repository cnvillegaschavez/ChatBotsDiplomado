using System.Collections.Generic;
using System.Linq;

namespace DiplomadoBot.App.Models
{
    public class OperationResult
    {
        public bool IsValid => Messages.All(r => r.Severity != 0);

        public List<OperationMessage> Messages { get; set; } = new List<OperationMessage>();
    }

    public class OperationResult<T> : OperationResult
    {
        public OperationResult()
        {
        }

        public OperationResult(T data)
        {
            Data = data;
        }

        public T Data { get; set; }
    }
}
