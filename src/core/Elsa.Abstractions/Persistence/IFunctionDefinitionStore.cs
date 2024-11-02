using Elsa.Models;
using Elsa.Persistence.Specifications;
using System.Threading.Tasks;
using System.Threading;

namespace Elsa.Persistence
{
    public interface IFunctionDefinitionStore : IStore<FunctionDefinition>
    {
        Task<FunctionDefinition?> FindAsync(string FunctionId, CancellationToken cancellationToken = default);
    }
}
