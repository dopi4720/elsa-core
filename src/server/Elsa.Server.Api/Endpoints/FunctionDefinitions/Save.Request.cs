using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elsa.Server.Api.Endpoints.FunctionDefinitions
{
    public partial class Save
    {
        public sealed record SaveFunctionDefinitionRequest
        {
            public string? Source { get; init; }
            public string? FunctionId { get; init; }
            public string? SampleInput { get; init; }
            public string? Catalog { get; init; }
            public string? FunctionType { get; init; }
        }
    }
}
