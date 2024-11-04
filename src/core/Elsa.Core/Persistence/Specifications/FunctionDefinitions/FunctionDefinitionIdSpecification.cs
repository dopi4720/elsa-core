using Elsa.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Elsa.Persistence.Specifications.FunctionDefinitions
{
  public  class FunctionDefinitionIdSpecification : Specification<FunctionDefinition>
    {
        public FunctionDefinitionIdSpecification(string functionId)
        {
            FunctionId = functionId;
        }

        public string FunctionId { get; set; }

        public override Expression<Func<FunctionDefinition, bool>> ToExpression()
        {
            Expression<Func<FunctionDefinition, bool>> predicate = x => x.Id.ToLower() == FunctionId.ToLower();
            return predicate;
        }
    }
}
