using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Models;
using Elsa.Services;
using Elsa.Services.Models;

namespace Elsa.Providers.Functions
{
    /// <summary>
    /// Represents a source of workflows for the <see cref="IFunctionRegistry"/>
    /// </summary>
    public interface IFunctionProvider
    {
        ValueTask<IEnumerable<FunctionDefinition>> ListAsync(string Name, string DisplayName, string SourceKeyword, CancellationToken cancellationToken = default);
        ValueTask<FunctionDefinition?> FindAsync(string FunctionId, CancellationToken cancellationToken = default);
        ValueTask<FunctionDefinition?> FindByDisplayNameAsync(string DisplayName, CancellationToken cancellationToken = default);
        ValueTask<IEnumerable<FunctionDefinition>> FindManyByNames(string DisplayName, CancellationToken cancellationToken = default);
    }
}