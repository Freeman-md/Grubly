using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Grubly.Data;
using Grubly.Interfaces.Repositories;
using Grubly.Models;
using Grubly.Repositories;
using Grubly.Tests.Unit.Builders;
using Grubly.Tests.Unit.Fixtures;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace Grubly.Tests.Unit.Repository;

public partial class RecipeRepositoryTests : IClassFixture<TestFixture>
{
    private readonly ServiceProvider _serviceProvider;

    public RecipeRepositoryTests(TestFixture fixture)
    {
        _serviceProvider = fixture.ServiceProvider;
    }

    private (IRecipeRepository recipeRepository, GrublyContext dbContext) CreateScope()
    {
        var scope = _serviceProvider.CreateScope();
        var scopedServices = scope.ServiceProvider;

        var recipeRepository = scopedServices.GetRequiredService<IRecipeRepository>();
        var dbContext = scopedServices.GetRequiredService<GrublyContext>();

        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        return (recipeRepository, dbContext);
    }

    [Fact]
    public async Task CreateRecipe_ValidInput_AddsRecipeToDatabase()
    {
        var (recipeRepository, dbContext) = CreateScope();

        #region Arrange
        Recipe unSavedRecipe = new RecipeBuilder().Build();
        #endregion

        #region Act
        Recipe savedRecipe = await recipeRepository.Create(unSavedRecipe);
        #endregion

        #region Assert
        Assert.NotNull(savedRecipe);
        Assert.True(savedRecipe.ID > 0, "The Recipe ID should be greater than 0 after saving to the database.");
        Assert.Equal(unSavedRecipe.Title, savedRecipe.Title);
        #endregion
    }

    [Fact]
    public async Task CreateRecipe_NullInput_ThrowsArgumentNullException()
    {
        var (recipeRepository, dbContext) = CreateScope();

        #region Arrange
        Recipe? nullRecipe = null;
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => recipeRepository.Create(nullRecipe!));
        #endregion
    }

    [Theory]
    [InlineData(null, "Valid Description", CuisineType.Italian, DifficultyLevel.Easy)]
    [InlineData("Valid Title", null, CuisineType.Italian, DifficultyLevel.Medium)]
    
    public async Task CreateRecipe_InvalidInputs_ThrowsDbUpdateException(string title, string description, CuisineType type, DifficultyLevel difficultyLevel)
    {
        var (recipeRepository, dbContext) = CreateScope();

        #region Arrange
        Recipe unSavedRecipe = new Recipe { Title = title, Description = description, CuisineType = type, DifficultyLevel = difficultyLevel };
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => recipeRepository.Create(unSavedRecipe));
        #endregion
    }

    // [Fact]
    // public async Task CreateRecipe_DuplicateEntity_ForSameUser_ThrowsArgumentException()
    // {
    //     // TODO: Refactor test when authentication has been implemented
    //     var (recipeRepository, dbContext) = CreateScope();

    //     #region Arrange
    //     Recipe originalRecipe = new RecipeBuilder().Build();
    //     Recipe duplicateRecipe = new Recipe
    //     {
    //         Title = originalRecipe.Title,
    //         Description = originalRecipe.Description,
    //         CuisineType = originalRecipe.CuisineType,
    //         DifficultyLevel = originalRecipe.DifficultyLevel,
    //     };
    //     #endregion

    //     #region Act
    //     await recipeRepository.Create(originalRecipe);
    //     #endregion

    //     #region Assert
    //     await Assert.ThrowsAsync<ArgumentException>(() => recipeRepository.Create(duplicateRecipe));
    //     #endregion
    // }

    [Fact]
    public async Task CreateRecipe_WithRelations_EnsuresCorrectForeignKeysAndSavesRelatedEntities()
    {
        var (recipeRepository, dbContext) = CreateScope();

        #region Arrange
        // Define ingredients
        Ingredient[] ingredients = {
        new Ingredient { Name = "Tomato", Description = "Fresh red tomatoes" },
        new Ingredient { Name = "Garlic", Description = "Fresh garlic cloves" }
    };

        // Define categories
        Category[] categories = {
        new Category { Name = "Breakfast" },
        new Category { Name = "Lunch" }
    };

        // Add ingredients and categories to the context
        dbContext.Ingredients.AddRange(ingredients);
        dbContext.Categories.AddRange(categories);
        await dbContext.SaveChangesAsync();

        // Create a new recipe with ingredients and categories
        Recipe unSavedRecipe = new RecipeBuilder()
                                    .WithTitle("Tomato Omelette")
                                    .WithIngredients(ingredients)
                                    .WithCategories(categories)
                                    .Build();
        #endregion

        #region Act
        // Add the recipe to the repository
        Recipe savedRecipe = await recipeRepository.Create(unSavedRecipe);
        #endregion

        #region Assert
        Assert.True(savedRecipe.ID > 0, "The Recipe ID should be greater than 0 after saving to the database.");

        // get model directly from db using repository to ensure relations were saved
        Recipe? retrievedRecipe = await recipeRepository.GetOneWithAllDetails(savedRecipe.ID);
        Assert.NotNull(retrievedRecipe);
        Assert.Equal(unSavedRecipe.Title, retrievedRecipe!.Title);

        // Verify that the recipe contains the correct ingredients
        Assert.Equal(ingredients.Length, retrievedRecipe.Ingredients!.Count);
        foreach (var ingredient in ingredients)
        {
            Assert.Contains(retrievedRecipe.Ingredients, i => i.Name == ingredient.Name && i.Description == ingredient.Description);
        }

        // Verify that the recipe contains the correct categories
        Assert.Equal(categories.Length, retrievedRecipe.Categories!.Count);
        foreach (var category in categories)
        {
            Assert.Contains(retrievedRecipe.Categories, c => c.Name == category.Name);
        }
        #endregion
    }


    [Fact]
    public async Task CreateRecipe_InvalidForeignKey_ThrowsDbUpdateException()
    {
        var (recipeRepository, dbContext) = CreateScope();

        #region Arrange
        var invalidIngredient = new Ingredient { ID = 999, Name = "Non-Existent Ingredient" }; // Invalid ID
        var invalidCategory = new Category { ID = 999, Name = "Non-Existent Category" }; // Invalid ID

        // Build the recipe with these non-existent entities
        Recipe unSavedRecipe = new RecipeBuilder()
            .WithTitle("Invalid Recipe")
            .WithIngredients(invalidIngredient)  // Invalid reference
            .WithCategories(invalidCategory)     // Invalid reference
            .Build();
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => recipeRepository.Create(unSavedRecipe));
        #endregion
    }
}
