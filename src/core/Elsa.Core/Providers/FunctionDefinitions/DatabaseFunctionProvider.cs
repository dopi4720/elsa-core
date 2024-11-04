using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Attributes;
using Elsa.Models;
using Elsa.Persistence;
using Elsa.Persistence.Specifications;
using Elsa.Persistence.Specifications.FunctionDefinitions;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.Extensions.Logging;

namespace Elsa.Providers.Functions
{
    /// <summary>
    /// Provides functions from the function definition store.
    /// </summary>
    [SkipTriggerIndexing]
    public class DatabaseFunctionProvider : FunctionProvider
    {
        private readonly IFunctionDefinitionStore _functionDefinitionStore;
        private readonly ILogger _logger;

        public DatabaseFunctionProvider(IFunctionDefinitionStore functionDefinitionStore, ILogger<DatabaseFunctionProvider> logger)
        {
            _functionDefinitionStore = functionDefinitionStore;
            _logger = logger;
        }

        public override async ValueTask<FunctionDefinition?> FindAsync(string FunctionId, CancellationToken cancellationToken = default)
        {
    var function =await        _functionDefinitionStore.FindAsync(new FunctionDefinitionIdSpecification(FunctionId), cancellationToken);
            return function;
        }

        public override async ValueTask<FunctionDefinition?> FindByDisplayNameAsync(string DisplayName, CancellationToken cancellationToken = default)
        {
            var function = await _functionDefinitionStore.FindAsync(new FunctionDefinitionDisplayNameSpecification(DisplayName), cancellationToken);
            return function;
        }

        public override async ValueTask<IEnumerable<FunctionDefinition>> FindManyByNames(string DisplayName, CancellationToken cancellationToken = default)
        {
            var function = await _functionDefinitionStore.FindManyAsync(new FunctionDefinitionDisplayNameSpecification(DisplayName), cancellationToken: cancellationToken);
            return function;
        }

        public override async ValueTask<IEnumerable<FunctionDefinition>> ListAsync(string Name, string DisplayName, string SourceKeyword, CancellationToken cancellationToken = default)
        {
            var function = await _functionDefinitionStore.FindManyAsync(new FunctionDefinitionKeywordSpecification(DisplayName, Name, SourceKeyword), cancellationToken: cancellationToken);
            return function;
        }
    }
}