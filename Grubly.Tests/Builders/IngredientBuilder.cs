using System;
using Grubly.Models;

namespace Grubly.Tests.Unit.Builders;

public class IngredientBuilder
{
    private Ingredient _ingredient;

    public IngredientBuilder()
    {
        _ingredient = new Ingredient
        {
            Name = Guid.NewGuid().ToString(),
        };
    }

    public IngredientBuilder WithId(int id)
    {
        _ingredient.ID = id;
        return this;
    }

    public IngredientBuilder WithName(string name)
    {
        _ingredient.Name = name;
        return this;
    }

    public IngredientBuilder WithDescription(string description)
    {
        _ingredient.Description = description;
        return this;
    }

    public IngredientBuilder WithRecipes(params Recipe[] recipes)
    {
        _ingredient.Recipes = recipes.ToList();
        return this;
    }

    public Ingredient Build()
    {
        return _ingredient;
    }

    public static List<Ingredient> BuildMany(int count)
    {
        var ingredients = new List<Ingredient>();
        for (int i = 0; i < count; i++)
        {
            ingredients.Add(new IngredientBuilder()
                .WithName(Guid.NewGuid().ToString())
                .WithRecipes(new RecipeBuilder().WithTitle(Guid.NewGuid().ToString()).Build(), new RecipeBuilder().WithTitle(Guid.NewGuid().ToString()).Build())
                .Build());
        }
        return ingredients;
    }
}

