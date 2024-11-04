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
        Expression<Func<FunctionDefinition, bool>> predicate = x => true; // Bắt đầu với điều kiện mặc định là true

        if (!string.IsNullOrWhiteSpace(DisplayName))
        {
            predicate = predicate.And(x => x.DisplayName.ToLower().Contains(DisplayName.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(Name))
        {
            predicate = predicate.And(x => x.Name.ToLower().Contains(Name.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(SourceKeyword))
        {
            predicate = predicate.And(x => x.Source.ToLower().Contains(SourceKeyword.ToLower()));
        }

        return predicate;
    }
}