using System.Linq;
using Elsa.Models;
using Elsa.Server.Api.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Elsa.Server.Api.Swagger.Examples
{
    public class FunctionDefinitionPagedListExample : IExamplesProvider<PagedList<FunctionDefinition>>
    {
        public PagedList<FunctionDefinition> GetExamples()
        {
            var workflowDefinitionExamples = Enumerable.Range(1, 3).Select(_ => new FunctionDefinitionExample().GetExamples()).ToList();
            return new PagedList<FunctionDefinition>(workflowDefinitionExamples, 0, 10, workflowDefinitionExamples.Count);
        }
    }
}