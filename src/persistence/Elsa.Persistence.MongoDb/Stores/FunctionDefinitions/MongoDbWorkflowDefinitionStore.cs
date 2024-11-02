using Elsa.Models;
using Elsa.Services;
using MongoDB.Driver;
using System.Threading;
using System.Threading.Tasks;

namespace Elsa.Persistence.MongoDb.Stores
{
    public class MongoDbFunctionDefinitionStore : MongoDbStore<FunctionDefinition>, IFunctionDefinitionStore
    {
        public MongoDbFunctionDefinitionStore(IMongoCollection<FunctionDefinition> collection, IIdGenerator idGenerator) : base(collection, idGenerator)
        {
        }

        public Task<FunctionDefinition?> FindAsync(string FunctionId, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}