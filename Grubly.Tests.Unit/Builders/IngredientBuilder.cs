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

    public IngredientBuilder WithName(string name)
    {
        _ingredient.Name = name;
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
                .WithRecipes(new RecipeBuilder().WithTitle("Tomato Omelette").Build(), new RecipeBuilder().WithTitle("Egg Yolk").Build())
                .Build());
        }
        return ingredients;
    }
}

