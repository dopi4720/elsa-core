using Elsa.Models;
using MediatR;

namespace Elsa.Events
{
    public abstract class FunctionDefinitionNotification : INotification
    {
        public FunctionDefinitionNotification(FunctionDefinition functionDefinition) => FunctionDefinition = functionDefinition;
        public FunctionDefinition FunctionDefinition { get; }
    }
}