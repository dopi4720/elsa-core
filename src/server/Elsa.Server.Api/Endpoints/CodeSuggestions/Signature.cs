using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Elsa.Models;
using Elsa.Persistence;
using Elsa.Persistence.Specifications;
using Elsa.Persistence.Specifications.FunctionDefinitions;
using Elsa.Server.Api.Endpoints.FunctionDefinitions.Configs;
using Elsa.Server.Api.Helpers;
using Elsa.Server.Api.Models;
using Elsa.Server.Api.Swagger.Examples;
using Elsa.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MonacoRoslynCompletionProvider;
using MonacoRoslynCompletionProvider.Api;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using static System.Net.Mime.MediaTypeNames;

namespace Elsa.Server.Api.Endpoints.CodeSuggestions
{
    [ApiController]
    [ApiVersion("1")]
    [Route("v{apiVersion:apiVersion}/code-analysis/signature")]
    [Produces("application/json")]
    public class Signature : Controller
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SignatureHelpResult))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(FunctionDefinitionPagedListExample))]
        [SwaggerOperation(
            Summary = "Returns signature for monaco editor.",
            Description = "Get signature for monaco editor",
            OperationId = "SignatureHelpResult",
            Tags = new[] { "CodeAnalysis" })
        ]
        public async Task<ActionResult<SignatureHelpResult>> Handle([FromBody] SignatureHelpRequest signatureHelpRequest)
        {
            try
            {
                signatureHelpRequest.Assemblies = FunctionDefinitionConfigs.NeedDllFiles.ToArray();
                var signatureHelpResult = await CompletitionRequestHandler.Handle(signatureHelpRequest);
                return Ok(signatureHelpResult);
            }
            catch (Exception ex)
            {
                return Json(ex, SerializationHelper.GetSettingsForEndpoint(new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
            }
        }
    }
}
