using Elsa.Models;
using Elsa.Persistence;
using Elsa.Persistence.Specifications.FunctionDefinitions;
using Elsa.Server.Api.Endpoints.FunctionDefinitions.Models;
using Elsa.Server.Api.Endpoints.FunctionDefinitions.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Elsa.Server.Api.Endpoints.FunctionDefinitions.Save;
using static Elsa.Server.Api.Endpoints.WorkflowDefinitions.Save;

namespace Elsa.Server.Api.Endpoints.FunctionDefinitions
{
    [ApiController]
    [ApiVersion("1")]
    [Route("v{apiVersion:apiVersion}/function-definitions")]
    [Produces("application/json")]
    public partial class Compile : Controller
    {
        [HttpPost("Compile")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(FunctionGeneralView))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(FunctionGeneralView))]
        [SwaggerOperation(
            Summary = "Compile function",
            Description =
                "Compile function",
            OperationId = "FunctionDefinitions.Post",
            Tags = new[] { "FunctionDefinitions" })
        ]
        public async Task<IActionResult> Handle([FromBody] SaveFunctionDefinitionRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.FunctionType) || (!request.FunctionType.Equals("Function") && !request.FunctionType.Equals("SharedUtility")))
            {
                return BadRequest(new FunctionGeneralView()
                {
                    IsSuccess = false,
                    Message = "Missing Function Type",
                    Data = null
                });
            }

            try
            {
                var compiled = DynamicCompiler.Compile(request.Source ?? throw new Exception("Source cannot be empty"));
                if (!compiled.IsCompiled)
                {
                    return BadRequest(new FunctionGeneralView()
                    {
                        IsSuccess = false,
                        Message = compiled.CompileMessage,
                        Data = null
                    });
                }

                return Ok(new FunctionGeneralView()
                {
                    IsSuccess = true,
                    Message = "Compile successfully",
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
