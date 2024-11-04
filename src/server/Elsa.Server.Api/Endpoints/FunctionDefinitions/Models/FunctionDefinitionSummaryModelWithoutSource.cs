using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elsa.Server.Api.Endpoints.FunctionDefinitions
{
    public record FunctionDefinitionSummaryModelWithoutSource(
        string Id,
        string? Name,
        string? DisplayName,
        int? Version,
        bool IsPublish);
}