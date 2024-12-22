using Elsa.Models;
using Elsa.Persistence;
using Elsa.Persistence.Specifications.FunctionDefinitions;
using Elsa.Server.Api.Endpoints.FunctionDefinitions.Configs;
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
    public partial class GetTemplate : Controller
    {
        [HttpGet("GetTemplate/{FunctionType}")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(FunctionGeneralView))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(FunctionGeneralView))]
        [SwaggerOperation(
            Summary = "Get function Template",
            Description =
                "Get function Template",
            OperationId = "FunctionDefinitions.Post",
            Tags = new[] { "FunctionDefinitions" })
        ]
        public async Task<IActionResult> Handle(string FunctionType, CancellationToken cancellationToken)
        {
            try
            {
                string FunctionTemplate = "";
                switch (FunctionType)
                {
                    case "Function":
                        FunctionTemplate = @$"using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DrpSystem.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;

public class <<<DRPClass>>>
{{
    //Author: HuyNT
    //Write at:
    public static async ValueTask<object?> Execute({FunctionDefinitionConfigs.DBContextName} drpContext)
    {{
        //Write code here
        return true;
    }}
}}";
                        break;
                    case "SharedUtility":
                        FunctionTemplate = @$"using System;

public class DRPClass
{{
    //Author: HuyNT
    //Write at:
    //Purpose:
}}";
                        break;
                    default:
                        break;
                }

                return Ok(new FunctionGeneralView()
                {
                    IsSuccess = true,
                    Message = "Sucessfully",
                    Data = FunctionTemplate
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
