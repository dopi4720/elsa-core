using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Elsa.Models;
using Elsa.Persistence.EntityFramework.Core.Services;
using Elsa.Persistence.Specifications;
using Elsa.Serialization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Elsa.Persistence.EntityFramework.Core.Stores.FunctionDefinitions
{
    internal class EntityFrameworkFunctionDefinitionStore :  ElsaContextEntityFrameworkStore<FunctionDefinition>, IFunctionDefinitionStore
    {
        private readonly IContentSerializer _contentSerializer;

        public EntityFrameworkFunctionDefinitionStore(IElsaContextFactory dbContextFactory, IMapper mapper, IContentSerializer contentSerializer, ILogger<EntityFrameworkFunctionDefinitionStore> logger) : base(dbContextFactory, mapper, logger)
        {
            _contentSerializer = contentSerializer;
        }

        protected override Expression<Func<FunctionDefinition, bool>> MapSpecification(ISpecification<FunctionDefinition> specification) => AutoMapSpecification(specification);

        protected override void OnSaving(ElsaContext dbContext, FunctionDefinition entity)
        {
            var data = new
            {
                entity.Name,
                entity.DisplayName,
                entity.FunctionType,
                entity.Catalog,
                entity.Source,
                entity.Binary,
                entity.Pdb,
                entity.Version,
                entity.IsPublish,
                entity.LastUpdate,
                entity.SampleInput
            };

            var json = _contentSerializer.Serialize(data);
            dbContext.Entry(entity).Property("Data").CurrentValue = json;
        }

        protected override void OnLoading(ElsaContext dbContext, FunctionDefinition entity)
        {
            //var data = new
            //{
            //    entity.Name,
            //    entity.DisplayName,
            //    entity.FunctionType,
            //    entity.Catalog,
            //    entity.Source,
            //    entity.Binary,
            //    entity.Pdb,
            //    entity.Version,
            //    entity.IsPublish,
            //    entity.LastUpdate,
            //    entity.SampleInput
            //};

            //var json = (string)dbContext.Entry(entity).Property("Data").CurrentValue;
            //data = JsonConvert.DeserializeAnonymousType(json, data, DefaultContentSerializer.CreateDefaultJsonSerializationSettings())!;

            //entity.Name = data.Name;
            //entity.DisplayName = data.DisplayName;
            //entity.FunctionType = data.FunctionType;
            //entity.Catalog = data.Catalog;
            //entity.Source = data.Source;
            //entity.Binary = data.Binary;
            //entity.Pdb = data.Pdb;
            //entity.Version = data.Version;
            //entity.IsPublish = data.IsPublish;
            //entity.LastUpdate = data.LastUpdate;
            //entity.SampleInput = data.SampleInput;
        }

        public Task<FunctionDefinition?> FindAsync(string FunctionId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
