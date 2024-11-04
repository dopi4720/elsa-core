using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Models;
using Elsa.Persistence.Specifications;
using Elsa.Services;

namespace Elsa.Persistence.Decorators
{
    public class InitializingFunctionDefinitionStore : IFunctionDefinitionStore
    {
        private readonly IFunctionDefinitionStore _store;
        private readonly IIdGenerator _idGenerator;

        public InitializingFunctionDefinitionStore(IFunctionDefinitionStore store, IIdGenerator idGenerator)
        {
            _store = store;
            _idGenerator = idGenerator;
        }

        public async Task SaveAsync(FunctionDefinition entity, CancellationToken cancellationToken)
        {
            entity = Initialize(entity);
            await _store.SaveAsync(entity, cancellationToken);
        }
        
        public async Task UpdateAsync(FunctionDefinition entity, CancellationToken cancellationToken)
        {
            entity = Initialize(entity);
            await _store.UpdateAsync(entity, cancellationToken);
        }

        public async Task AddAsync(FunctionDefinition entity, CancellationToken cancellationToken = default)
        {
            entity = Initialize(entity);
            await _store.AddAsync(entity, cancellationToken);
        }

        public async Task AddManyAsync(IEnumerable<FunctionDefinition> entities, CancellationToken cancellationToken = default)
        {
            var list = entities.ToList();

            foreach (var entity in list)
                Initialize(entity);
            
            await _store.AddManyAsync(list, cancellationToken);
        }

        public Task DeleteAsync(FunctionDefinition entity, CancellationToken cancellationToken) => _store.DeleteAsync(entity, cancellationToken);
        public Task<int> DeleteManyAsync(ISpecification<FunctionDefinition> specification, CancellationToken cancellationToken) => _store.DeleteManyAsync(specification, cancellationToken);

        public Task<IEnumerable<FunctionDefinition>> FindManyAsync(
            ISpecification<FunctionDefinition> specification,
            IOrderBy<FunctionDefinition>? orderBy,
            IPaging? paging,
            CancellationToken cancellationToken) =>
            _store.FindManyAsync(specification, orderBy, paging, cancellationToken);

        public Task<int> CountAsync(ISpecification<FunctionDefinition> specification, CancellationToken cancellationToken) => _store.CountAsync(specification, cancellationToken);

        public Task<FunctionDefinition?> FindAsync(ISpecification<FunctionDefinition> specification, CancellationToken cancellationToken) => _store.FindAsync(specification, cancellationToken);

        
        private FunctionDefinition Initialize(FunctionDefinition functionDefinition)
        {
            if (string.IsNullOrWhiteSpace(functionDefinition.Id))
                functionDefinition.Id = _idGenerator.Generate();

            if (functionDefinition.Version == 0)
                functionDefinition.Version = 1;

            if (string.IsNullOrWhiteSpace(functionDefinition.Id))
                functionDefinition.Id = _idGenerator.Generate();

            return functionDefinition;
        }
    }
}