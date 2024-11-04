using Elsa.Models;

namespace Elsa.Events
{
    public class FunctionDefinitionPublishing : FunctionDefinitionNotification
    {
        public FunctionDefinitionPublishing(FunctionDefinition functionDefinition) : base(functionDefinition)
        {
        }
    }
}