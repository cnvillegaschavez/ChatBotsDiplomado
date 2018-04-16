namespace DiplomadoBot.App.Models
{
    public class OperationMessage
    {
        public OperationMessage(string propertyName, string errorMessage)
        {
            Property = propertyName;
            Description = errorMessage;
        }

        public OperationMessage(string propertyName, string errorMessage, int severity)
        {
            Property = propertyName;
            Description = errorMessage;
            Severity = severity;
        }

        public OperationMessage()
        {
        }

        public string Property { get; set; }

        public string Description { get; set; }

        public int Severity { get; set; }
    }
}
