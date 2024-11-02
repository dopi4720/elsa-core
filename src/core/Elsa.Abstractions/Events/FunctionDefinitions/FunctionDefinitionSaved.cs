using Elsa.Models;

namespace Elsa.Events
{
    public class FunctionDefinitionSaved : FunctionDefinitionNotification
    {
        public FunctionDefinitionSaved(FunctionDefinition functionDefinition) : base(functionDefinition)
        {
        }
    }
}