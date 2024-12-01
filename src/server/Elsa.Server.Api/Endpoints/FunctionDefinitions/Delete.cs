using Elsa.Models;
using Elsa.Persistence;
using Elsa.Persistence.Specifications.FunctionDefinitions;
using Elsa.Server.Api.Endpoints.FunctionDefinitions.Models;
using Elsa.Server.Api.Endpoints.FunctionDefinitions.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Elsa.Server.Api.Endpoints.WorkflowDefinitions.Save;

namespace Elsa.Server.Api.Endpoints.FunctionDefinitions
{
    [ApiController]
    [ApiVersion("1")]
    [Route("v{apiVersion:apiVersion}/function-definitions/")]
    [Produces("application/json")]
    public partial class Delete : Controller
    {
        private readonly IFunctionDefinitionStore _functionDefinitionStore;
        public Delete(IFunctionDefinitionStore functionDefinitionStore)
        {
            _functionDefinitionStore = functionDefinitionStore;
        }

        [HttpPost("delete/{FunctionId}")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(FunctionGeneralView))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(FunctionGeneralView))]
        [SwaggerOperation(
            Summary = "Delete function definition",
            Description =
                "Delete function definition",
            OperationId = "FunctionDefinitions.Post",
            Tags = new[] { "FunctionDefinitions" })
        ]
        public async Task<IActionResult> Handle(string FunctionId, CancellationToken cancellationToken)
        {
            try
            {
                await _functionDefinitionStore.DeleteManyAsync(new FunctionDefinitionFunctionIdSpecification(FunctionId ?? ""), cancellationToken);
                return Ok(new FunctionGeneralView()
                {
                    IsSuccess = true,
                    Message = "Delete function successfully",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new FunctionGeneralView()
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}
