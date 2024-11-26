using Elsa.Models;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Elsa.Persistence.Specifications.FunctionDefinitions
{
    public class FunctionDefinitionFunctionIdSpecification : Specification<FunctionDefinition>
    {
        public FunctionDefinitionFunctionIdSpecification(string functionId)
        {
            FunctionId = functionId;
        }

        public string FunctionId { get; set; }

        public override Expression<Func<FunctionDefinition, bool>> ToExpression()
        {
            return x => x.FunctionId.ToLower() == FunctionId.ToLower();
        }

        public FunctionDefinition GetLatestVersion(IEnumerable<FunctionDefinition> definitions)
        {
            var filteredDefinitions = definitions.AsQueryable().Where(ToExpression());
            return filteredDefinitions.OrderByDescending(x => x.Version).FirstOrDefault();
        }
    }
}
