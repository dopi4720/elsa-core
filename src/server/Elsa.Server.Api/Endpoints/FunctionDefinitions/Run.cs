using Elsa.Models;
using Elsa.Persistence;
using Elsa.Persistence.Specifications.FunctionDefinitions;
using Elsa.Server.Api.Endpoints.FunctionDefinitions.Models;
using Elsa.Server.Api.Endpoints.FunctionDefinitions.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
using static Elsa.Server.Api.Endpoints.FunctionDefinitions.Save;
using static Elsa.Server.Api.Endpoints.WorkflowDefinitions.Save;

namespace Elsa.Server.Api.Endpoints.FunctionDefinitions
{
    [ApiController]
    [ApiVersion("1")]
    [Route("v{apiVersion:apiVersion}/function-definitions")]
    [Produces("application/json")]
    public partial class Run : Controller
    {
        [HttpPost("RunFunction")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(FunctionGeneralView))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(FunctionGeneralView))]
        [SwaggerOperation(
            Summary = "Run function",
            Description =
                "Run function",
            OperationId = "FunctionDefinitions.Post",
            Tags = new[] { "FunctionDefinitions" })
        ]
        public async Task<IActionResult> Handle([FromBody] SaveFunctionDefinitionRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.FunctionType) || !request.FunctionType.Equals("Function"))
            {
                return BadRequest(new FunctionGeneralView()
                {
                    IsSuccess = false,
                    Message = "Only run Function",
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

                var function = DynamicCompiler.Compile(request.Source);
                var result = await DynamicRunner.RunAsync(function.DllBytes, function.PdbBytes, function.ClassName, JsonConvert.DeserializeObject<Dictionary<string, object?>>(request.SampleInput));

                return Ok(new FunctionGeneralView()
                {
                    IsSuccess = true,
                    Message = "Save function successfully",
                    Data = result
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
