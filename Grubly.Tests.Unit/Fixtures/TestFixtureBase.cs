using System;
using Grubly.Data;
using Grubly.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Grubly.Tests.Unit.Fixtures;

public class TestFixtureBase
{
    public ServiceProvider ServiceProvider { get; private set; }
    public GrublyContext DbContext { get; private set; }

    public TestFixtureBase()
    {
        var services = new ServiceCollection();

        services.AddDbContext<GrublyContext>(
            options => options.UseInMemoryDatabase("GrublyTests")
        );

        ServiceProvider = services.BuildServiceProvider();

        DbContext = ServiceProvider.GetRequiredService<GrublyContext>();

        ResetDatabase();
    }

    public async Task SeedData(int numIngredients = 2, int numCategories = 2, int numRecipes = 2)
    {
        var ingredients = new List<Ingredient>();
        var categories = new List<Category>();

        // Generate Ingredients
        for (int i = 1; i <= numIngredients; i++)
        {
            ingredients.Add(new Ingredient
            {
                Name = $"Ingredient {i}",
                Description = $"Description for Ingredient {i}"
            });
        }

        // Generate Categories
        for (int i = 1; i <= numCategories; i++)
        {
            categories.Add(new Category
            {
                Name = $"Category {i}"
            });
        }

        // Generate Recipes
        var recipes = new List<Recipe>();
        for (int i = 1; i <= numRecipes; i++)
        {
            recipes.Add(new Recipe
            {
                Title = $"Recipe {i}",
                Description = $"A simple and delicious Recipe {i}.",
                Instructions = $"1. Step 1 for Recipe {i}\n2. Step 2 for Recipe {i}",
                CuisineType = CuisineType.Italian,
                DifficultyLevel = DifficultyLevel.Easy,
                ImageUrl = $"https://example.com/recipe_{i}.jpg",
                Ingredients = ingredients.Take(2).ToList(),  // Use first two ingredients
                Categories = categories.Take(1).ToList()  // Use first category
            });
        }

        // Add to DbContext
        DbContext.Ingredients.AddRange(ingredients);
        DbContext.Categories.AddRange(categories);
        DbContext.Recipes.AddRange(recipes);

        await DbContext.SaveChangesAsync();
    }


    public void ResetDatabase()
    {
        DbContext.Database.EnsureDeleted();
        DbContext.Database.EnsureCreated();
    }

}
