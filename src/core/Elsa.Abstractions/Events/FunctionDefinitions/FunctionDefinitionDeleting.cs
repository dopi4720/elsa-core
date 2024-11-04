using Elsa.Models;

namespace Elsa.Events
{
    public class FunctionDefinitionDeleting : FunctionDefinitionNotification
    {
        public FunctionDefinitionDeleting(FunctionDefinition functionDefinition) : base(functionDefinition)
        {
        }
    }
}