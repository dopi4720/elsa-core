using Elsa.Models;

namespace Elsa.Events
{
    public class FunctionDefinitionSaving : FunctionDefinitionNotification
    {
        public FunctionDefinitionSaving(FunctionDefinition functionDefinition) : base(functionDefinition)
        {
        }
    }
}