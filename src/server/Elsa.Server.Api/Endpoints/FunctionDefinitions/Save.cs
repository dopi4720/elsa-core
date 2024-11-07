using Elsa.Models;
using Elsa.Persistence;
using Elsa.Server.Api.Endpoints.FunctionDefinitions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Elsa.Server.Api.Endpoints.WorkflowDefinitions.Save;

namespace Elsa.Server.Api.Endpoints.FunctionDefinitions
{
    [ApiController]
    [ApiVersion("1")]
    [Route("v{apiVersion:apiVersion}/function-definitions")]
    [Produces("application/json")]
    public partial class Save : Controller
    {
        private readonly IFunctionDefinitionStore _functionDefinitionStore;
        public Save(IFunctionDefinitionStore functionDefinitionStore)
        {
            _functionDefinitionStore = functionDefinitionStore;
        }

        [HttpPost("Save")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(FunctionGeneralView))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(FunctionGeneralView))]
        [SwaggerOperation(
            Summary = "Creates a new function definition or updates an existing one.",
            Description =
                "Creates a new function definition or function an existing one. If the function already exists, a new draft is created and updated with the specified values. Use the Publish field to automatically publish the workflow.",
            OperationId = "FunctionDefinitions.Post",
            Tags = new[] { "FunctionDefinitions" })
        ]
        public async Task<IActionResult> Handle([FromBody] SaveFunctionDefinitionRequest request, CancellationToken cancellationToken)
        {
            //TODO: Handle compile source code
            return Ok();
        }
    }
}
