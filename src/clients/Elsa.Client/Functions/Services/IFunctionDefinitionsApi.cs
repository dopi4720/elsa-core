using System.Threading;
using System.Threading.Tasks;
using Elsa.Client.Functions.Models;
using Elsa.Client.Models;
using Refit;

namespace Elsa.Client.Functions.Services
{
    public interface IFunctionDefinitionsApi
    {
        [Get("/v1/function-definitions/{functionDefinitionId}/{versionOptions}")]
        Task<FunctionDefinition> GetByIdAsync(string functionDefinitionId, VersionOptions versionOptions, CancellationToken cancellationToken = default);

        [Get("/v1/function-definitions/{functionDefinitionVersionId}")]
        Task<FunctionDefinition> GetByVersionIdAsync(string functionDefinitionVersionId, CancellationToken cancellationToken = default);

        //[Get("/v1/function-definitions")]
        //Task<PagedList<FunctionDefinitionSummary>> ListAsync(int? page = default, int? pageSize = default, VersionOptions? version = default, CancellationToken cancellationToken = default);

        //[Post("/v1/function-definitions")]
        //Task<FunctionDefinition> SaveAsync([Body] SaveFunctionDefinitionRequest request, CancellationToken cancellationToken = default);

        //[Delete("/v1/function-definitions/{functionDefinitionId}")]
        //Task DeleteAsync(string functionDefinitionId, CancellationToken cancellationToken = default);

        //[Delete("/v1/function-definitions/{functionDefinitionId}/{versionOptions}")]
        //Task DeleteAsync(string functionDefinitionId, VersionOptions versionOptions, CancellationToken cancellationToken = default);
    }
}