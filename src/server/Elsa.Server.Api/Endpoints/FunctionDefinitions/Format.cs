using Elsa.Models;
using Elsa.Persistence;
using Elsa.Persistence.Specifications.FunctionDefinitions;
using Elsa.Server.Api.Endpoints.FunctionDefinitions.Models;
using Elsa.Server.Api.Endpoints.FunctionDefinitions.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis;
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
    public partial class Format : Controller
    {
        [HttpPost("Format")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(FunctionGeneralView))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(FunctionGeneralView))]
        [SwaggerOperation(
            Summary = "Format function",
            Description =
                "Format function",
            OperationId = "FunctionDefinitions.Post",
            Tags = new[] { "FunctionDefinitions" })
        ]
        public async Task<IActionResult> Handle([FromBody] SaveFunctionDefinitionRequest request, CancellationToken cancellationToken)
        {
            try
            {
                string FormattedSourceCode = FormatCode(request.Source ?? "");

                return Ok(new FunctionGeneralView()
                {
                    IsSuccess = true,
                    Message = "Compile successfully",
                    Data = FormattedSourceCode
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
        string FormatCode(string code)
        {
            // Tạo syntax tree từ code input
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var root = syntaxTree.GetRoot();

            // Sử dụng workspace mặc định
            using (var workspace = new AdhocWorkspace())
            {
                // Thêm các tùy chọn format
                var options = workspace.Options
                    .WithChangedOption(FormattingOptions.IndentationSize, LanguageNames.CSharp, 4)
                    .WithChangedOption(FormattingOptions.UseTabs, LanguageNames.CSharp, false);

                // Format code
                var formattedRoot = Formatter.Format(root, workspace, options);
                return formattedRoot.ToFullString();
            }
        }
    }
}
