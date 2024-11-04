using Elsa.Models;

using LinqKit;

using System;
using System.Linq.Expressions;

namespace Elsa.Persistence.Specifications.FunctionDefinitions;

public class FunctionDefinitionDisplayNameSpecification : Specification<FunctionDefinition>
{
    public FunctionDefinitionDisplayNameSpecification(string displayName)
    {
        DisplayName = displayName;
    }

    public string DisplayName { get; set; }

    public override Expression<Func<FunctionDefinition, bool>> ToExpression()
    {
        Expression<Func<FunctionDefinition, bool>> predicate = x => x.DisplayName.ToLower().Contains(DisplayName.ToLower());
        return predicate;
    }
}