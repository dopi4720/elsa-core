using Elsa.Models;

namespace Elsa.Events
{
    public class FunctionDefinitionDeleted : FunctionDefinitionNotification
    {
        public FunctionDefinitionDeleted(FunctionDefinition functionDefinition) : base(functionDefinition)
        {
        }
    }
}