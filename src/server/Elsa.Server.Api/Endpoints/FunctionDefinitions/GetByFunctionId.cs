using AutoMapper;
using Elsa.Models;
using Elsa.Persistence;
using Elsa.Persistence.Specifications.FunctionDefinitions;
using Elsa.Server.Api.Helpers;
using Elsa.Server.Api.Swagger.Examples;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Elsa.Server.Api.Endpoints.FunctionDefinitions
{
    [ApiController]
    [ApiVersion("1")]
    [Route("v{apiVersion:apiVersion}/function-definitions/{functionId}")]
    [Produces("application/json")]
    public class GetByFunctionId : Controller
    {
        private readonly IFunctionDefinitionStore _functionDefinitionStore;
        private readonly IMapper _mapper;

        public GetByFunctionId(IFunctionDefinitionStore functionDefinitionStore, IMapper mapper)
        {
            _functionDefinitionStore = functionDefinitionStore;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FunctionDefinitionSummaryModelWithSource))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(FunctionDefinitionExample))]
        [SwaggerOperation(
            Summary = "Returns function definition detail.",
            Description = "Returns function definition detail",
            OperationId = "FunctionDefinitions.Detail",
            Tags = new[] { "FunctionDefinitions" })
        ]
        public async Task<IActionResult> Handle(string functionId, CancellationToken cancellationToken = default)
        {
            var functions = await _functionDefinitionStore.FindManyAsync(new FunctionDefinitionFunctionIdSpecification(functionId), cancellationToken: cancellationToken);
            if (functions == null || !functions.Any())
            {
                return NotFound();
            }
            var function = functions.OrderByDescending(x => x.Version).First();
            var FunctionSummary = _mapper.Map<FunctionDefinitionSummaryModelWithSource>(function);
            return function == null ? NotFound() : Json(FunctionSummary, SerializationHelper.GetSettingsForEndpoint(new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }
    }
}
