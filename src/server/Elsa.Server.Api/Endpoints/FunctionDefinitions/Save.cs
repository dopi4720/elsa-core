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

                var CurrentFunctions = await _functionDefinitionStore.FindManyAsync(new FunctionDefinitionFunctionIdSpecification(request.FunctionId ?? ""));
                var CurrentFunction = CurrentFunctions.OrderByDescending(x => x.Version).FirstOrDefault();

                int CurrentVersion = 1;
                if (CurrentFunction != null)
                {
                    CurrentVersion = CurrentFunction.Version + 1;
                }

                FunctionDefinition function = new()
                {
                    Source = request.Source,
                    Binary = compiled.DllBytes,
                    Catalog = !string.IsNullOrWhiteSpace(request.Catalog) ? request.Catalog : "Function",
                    DisplayName = compiled.ClassName ?? throw new Exception("Display name cannot be empty"),
                    FunctionType = request.FunctionType ?? throw new Exception("Function Type cannot be empty"),
                    IsPublish = true,
                    Id = Guid.NewGuid().ToString(),
                    FunctionId = string.IsNullOrWhiteSpace(request.FunctionId) ? Guid.NewGuid().ToString() : request.FunctionId,
                    LastUpdate = DateTime.Now,
                    Name = compiled.ClassName ?? throw new Exception("Name cannot be empty"),
                    Pdb = compiled.PdbBytes,
                    SampleInput = !string.IsNullOrWhiteSpace(request.SampleInput)? JToken.Parse(request.SampleInput).ToString(Newtonsoft.Json.Formatting.None) : "{}",
                    Version = CurrentVersion
                };

                await _functionDefinitionStore.SaveAsync(function, cancellationToken);
                return Ok(new FunctionGeneralView()
                {
                    IsSuccess = true,
                    Message = "Save function successfully",
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
