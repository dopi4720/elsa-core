using System.Collections.Generic;
using Elsa.Models;
using MediatR;

namespace Elsa.Events
{
    public class ManyFunctionDefinitionsDeleting : INotification
    {
        public ManyFunctionDefinitionsDeleting(IEnumerable<FunctionDefinition> functionDefinitions) => FunctionDefinitions = functionDefinitions;
        public IEnumerable<FunctionDefinition> FunctionDefinitions { get; }
    }
}