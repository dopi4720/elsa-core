using System;
using System.Collections.Generic;
using Elsa.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Elsa.Server.Api.Swagger.Examples
{
    public class FunctionDefinitionExample : IExamplesProvider<FunctionDefinition>
    {
        public FunctionDefinition GetExamples()
        {
            return new()
            {
                Name = "ProcessOrderFunction",
                DisplayName = "Process Order Function",
                Version = 1,
                Binary= new byte[] {},
                Pdb = new byte[] { },
                IsPublish = true,
                LastUpdate = DateTime.Now,
                SampleInput = "{}",
                Id = Guid.NewGuid().ToString(),
                FunctionType = "Type",
                Catalog = "Catalog",
                Source = "Source"
            };
        }
    }
}