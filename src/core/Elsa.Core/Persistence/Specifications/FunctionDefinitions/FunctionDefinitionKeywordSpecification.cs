using Elsa.Models;

using LinqKit;

using System;
using System.Linq.Expressions;

namespace Elsa.Persistence.Specifications.FunctionDefinitions;

public class FunctionDefinitionKeywordSpecification : Specification<FunctionDefinition>
{
    public FunctionDefinitionKeywordSpecification(string displayName, string name, string source)
    {
        DisplayName = displayName;
        Name = name;
        SourceKeyword = source;
    }

    public string DisplayName { get; set; }
    public string Name { get; set; }
    public string SourceKeyword { get; set; }

    public override Expression<Func<FunctionDefinition, bool>> ToExpression()
    {
        Expression<Func<FunctionDefinition, bool>> predicate = x => x.DisplayName.Contains(DisplayName);
        predicate = predicate.Or(x => x.Name.Contains(Name));
        predicate = predicate.Or(x => x.Source.Contains(SourceKeyword));
        return predicate;
    }
}