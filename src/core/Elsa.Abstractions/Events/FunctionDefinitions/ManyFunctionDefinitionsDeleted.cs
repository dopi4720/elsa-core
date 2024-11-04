using System.Collections.Generic;
using Elsa.Models;
using MediatR;

namespace Elsa.Events
{
    public class ManyFunctionDefinitionsDeleted : INotification
    {
        public ManyFunctionDefinitionsDeleted(IEnumerable<FunctionDefinition> functionDefinitions) => FunctionDefinitions = functionDefinitions;
        public IEnumerable<FunctionDefinition> FunctionDefinitions { get; }
    }
}