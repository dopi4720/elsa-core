using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Events;
using Elsa.Models;
using Elsa.Persistence.Specifications;
using MediatR;
using Open.Linq.AsyncExtensions;

namespace Elsa.Persistence.Decorators
{
    public class EventPublishingFunctionDefinitionStore : IFunctionDefinitionStore
    {
        private readonly IFunctionDefinitionStore _store;
        private readonly IMediator _mediator;

        public EventPublishingFunctionDefinitionStore(IFunctionDefinitionStore store, IMediator mediator)
        {
            _store = store;
            _mediator = mediator;
        }

        public Task<int> CountAsync(ISpecification<FunctionDefinition> specification, CancellationToken cancellationToken = default) => _store.CountAsync(specification, cancellationToken);

        public async Task DeleteAsync(FunctionDefinition entity, CancellationToken cancellationToken = default)
        {
            await _mediator.Publish(new FunctionDefinitionDeleting(entity), cancellationToken);
            await _store.DeleteAsync(entity, cancellationToken);
            await _mediator.Publish(new FunctionDefinitionDeleted(entity), cancellationToken);
        }

        public async Task<int> DeleteManyAsync(ISpecification<FunctionDefinition> specification, CancellationToken cancellationToken = default)
        {
            var functionDefinitions = await FindManyAsync(specification, cancellationToken: cancellationToken).ToList();

            if (!functionDefinitions.Any())
                return 0;

            foreach (var functionDefinition in functionDefinitions)
                await _mediator.Publish(new FunctionDefinitionDeleting(functionDefinition), cancellationToken);

            await _mediator.Publish(new ManyFunctionDefinitionsDeleting(functionDefinitions), cancellationToken);

            var count = await _store.DeleteManyAsync(specification, cancellationToken);

            foreach (var instance in functionDefinitions)
                await _mediator.Publish(new FunctionDefinitionDeleted(instance), cancellationToken);

            await _mediator.Publish(new ManyFunctionDefinitionsDeleted(functionDefinitions), cancellationToken);

            return count;
        }

        public Task<FunctionDefinition?> FindAsync(ISpecification<FunctionDefinition> specification, CancellationToken cancellationToken = default) => _store.FindAsync(specification, cancellationToken);

        public Task<IEnumerable<FunctionDefinition>> FindManyAsync(ISpecification<FunctionDefinition> specification, IOrderBy<FunctionDefinition>? orderBy = null, IPaging? paging = null, CancellationToken cancellationToken = default)
            => _store.FindManyAsync(specification, orderBy, paging, cancellationToken);

        public async Task SaveAsync(FunctionDefinition entity, CancellationToken cancellationToken = default)
        {
            await _mediator.Publish(new FunctionDefinitionSaving(entity), cancellationToken);
            await _store.SaveAsync(entity, cancellationToken);
            await _mediator.Publish(new FunctionDefinitionSaved(entity), cancellationToken);
        }

        public async Task AddAsync(FunctionDefinition entity, CancellationToken cancellationToken = default)
        {
            await _mediator.Publish(new FunctionDefinitionSaving(entity), cancellationToken);
            await _store.AddAsync(entity, cancellationToken);
            await _mediator.Publish(new FunctionDefinitionSaved(entity), cancellationToken);
        }

        public async Task AddManyAsync(IEnumerable<FunctionDefinition> entities, CancellationToken cancellationToken = default)
        {
            var list = entities.ToList();

            foreach (var entity in list)
                await _mediator.Publish(new FunctionDefinitionSaving(entity), cancellationToken);

            await _store.AddManyAsync(list, cancellationToken);

            foreach (var entity in list)
                await _mediator.Publish(new FunctionDefinitionSaved(entity), cancellationToken);
        }

        public async Task UpdateAsync(FunctionDefinition entity, CancellationToken cancellationToken = default)
        {
            await _mediator.Publish(new FunctionDefinitionSaving(entity), cancellationToken);
            await _store.UpdateAsync(entity, cancellationToken);
            await _mediator.Publish(new FunctionDefinitionSaved(entity), cancellationToken);
        }
    }
}