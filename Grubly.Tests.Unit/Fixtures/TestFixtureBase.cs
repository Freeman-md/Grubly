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

    public async Task SeedData()
    {
        var ingredient1 = new Ingredient { Name = "Tomato", Description = "Fresh red tomatoes" };
        var ingredient2 = new Ingredient { Name = "Garlic", Description = "Fresh garlic cloves" };
        var category = new Category { Name = "Breakfast" };
        var category2 = new Category { Name = "Lunch" };

        var recipe1 = new Recipe
        {
            Title = "Tomato Omelette",
            Description = "A simple and delicious tomato omelette.",
            Instructions = "1. Beat the eggs.\n2. Chop tomatoes and garlic.\n3. Heat olive oil in a pan.\n4. Sauté garlic and tomatoes.\n5. Pour eggs and cook until done.",
            CuisineType = CuisineType.Italian,
            DifficultyLevel = DifficultyLevel.Easy,
            ImageUrl = "https://example.com/tomato_omelette.jpg",
            Ingredients = new List<Ingredient> { ingredient1, ingredient2 },
            Categories = new List<Category> { category }
        };

        var recipe2 = new Recipe
        {
            Title = "Garlic Pasta",
            Description = "Delicious garlic pasta with olive oil and fresh herbs.",
            Instructions = "1. Boil the pasta.\n2. Heat olive oil in a pan.\n3. Add garlic and sauté.\n4. Mix pasta with garlic oil.\n5. Serve with herbs.",
            CuisineType = CuisineType.Italian,
            DifficultyLevel = DifficultyLevel.Medium,
            ImageUrl = "https://example.com/garlic_pasta.jpg",
            Ingredients = new List<Ingredient> { ingredient2 },
            Categories = new List<Category> { category2 }
        };

        DbContext.Ingredients.AddRange(ingredient1, ingredient2);
        DbContext.Categories.AddRange(category, category2);
        DbContext.Recipes.AddRange(recipe1, recipe2);

        await DbContext.SaveChangesAsync();
    }

    public void ResetDatabase()
    {
        DbContext.Database.EnsureDeleted();
        DbContext.Database.EnsureCreated();
    }

}
