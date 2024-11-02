using Elsa.Models;
using Elsa.Services;
using Microsoft.Extensions.Caching.Memory;
using System.Threading;
using System.Threading.Tasks;

namespace Elsa.Persistence.InMemory
{
    public class InMemoryFunctionDefinitionStore : InMemoryStore<FunctionDefinition>, IFunctionDefinitionStore
    {
        public InMemoryFunctionDefinitionStore(IMemoryCache memoryCache, IIdGenerator idGenerator) : base(memoryCache, idGenerator)
        {
        }

        public Task<FunctionDefinition?> FindAsync(string FunctionId, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}