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
using Elsa.Server.Api.Helpers;
using Elsa.Server.Api.Models;
using Elsa.Server.Api.Swagger.Examples;
using Elsa.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Elsa.Server.Api.Endpoints.FunctionDefinitions
{
    [ApiController]
    [ApiVersion("1")]
    [Route("v{apiVersion:apiVersion}/function-definitions")]
    [Produces("application/json")]
    public class List : Controller
    {
        private readonly IFunctionDefinitionStore _functionDefinitionStore;
        private readonly IMapper _mapper;

        public List(IFunctionDefinitionStore functionDefinitionStore, IMapper mapper)
        {
            _functionDefinitionStore = functionDefinitionStore;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedList<FunctionDefinitionSummaryModel>))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(FunctionDefinitionPagedListExample))]
        [SwaggerOperation(
            Summary = "Returns a list of function definitions.",
            Description = "Returns paginated a list of function definition summaries. When no version options are specified, the latest versions are returned.",
            OperationId = "FunctionDefinitions.List",
            Tags = new[] { "FunctionDefinitions" })
        ]
        public async Task<ActionResult<PagedList<FunctionDefinitionSummaryModel>>> Handle(
            [FromQuery] string? DisplayName = "",
            [FromQuery] string? Name = "",
            [FromQuery] string? SourceKeyword = "",
            [FromQuery] SortBy? sortBy = SortBy.Ascending,
            int? page = default,
            int? pageSize = default,
            CancellationToken cancellationToken = default)
        {
            var specification = new FunctionDefinitionKeywordSpecification(DisplayName ?? "", Name ?? "", SourceKeyword ?? "");
            var totalCount = await _functionDefinitionStore.CountAsync(specification, cancellationToken);
            var paging = page == null || pageSize == null ? default : Paging.Page(page.Value, pageSize.Value);
            var items = await _functionDefinitionStore.FindManyAsync(specification, default, paging, cancellationToken);
            var summaries = _mapper.Map<IList<FunctionDefinition>>(items);
            var pagedList = new PagedList<FunctionDefinition>(summaries, page, pageSize, totalCount);
            
            return Json(pagedList, SerializationHelper.GetSettingsForEndpoint(new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }
    }
}