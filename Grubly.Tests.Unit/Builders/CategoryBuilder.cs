using System;
using Grubly.Models;

namespace Grubly.Tests.Unit.Builders;

public class CategoryBuilder
{
 private Category _category;

    public CategoryBuilder()
    {
        _category = new Category
        {
            Name = Guid.NewGuid().ToString(),
        };
    }

    public CategoryBuilder WithName(string name)
    {
        _category.Name = name;
        return this;
    }

    public CategoryBuilder WithRecipes(params Recipe[] recipes)
    {
        _category.Recipes = recipes.ToList();
        return this;
    }

    public Category Build()
    {
        return _category;
    }

    public static List<Category> BuildMany(int count)
    {
        var categories = new List<Category>();
        for (int i = 0; i < count; i++)
        {
            categories.Add(new CategoryBuilder()
                .WithName(Guid.NewGuid().ToString())
                .WithRecipes(new RecipeBuilder().WithTitle("Tomato Omelette").Build(), new RecipeBuilder().WithTitle("Egg Yolk").Build())
                .Build());
        }
        return categories;
    }
}
