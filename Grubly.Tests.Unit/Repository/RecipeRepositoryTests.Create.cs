using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Grubly.Data;
using Grubly.Models;
using Grubly.Repositories;
using Grubly.Tests.Unit.Builders;
using Grubly.Tests.Unit.Fixtures;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Grubly.Tests.Unit.Repository;

public partial class RecipeRepositoryTests : IClassFixture<TestFixtureBase>
{
    private readonly TestFixtureBase _fixture;
    private readonly GrublyContext _dbContext;
    private readonly RecipeRepository _recipeRepository;

    private readonly IngredientRepository _ingredientRepository;
    private readonly CategoryRepository _categoryRepository;

    public RecipeRepositoryTests(TestFixtureBase fixture)
    {
        _fixture = fixture;

        _dbContext = fixture.DbContext;
        _recipeRepository = new RecipeRepository(_dbContext);
        _ingredientRepository = new IngredientRepository(_dbContext);
        _categoryRepository = new CategoryRepository(_dbContext);
    }

    [Fact]
    public async Task CreateRecipe_ValidInput_AddsRecipeToDatabase()
    {
        #region Arrange
        Recipe unSavedRecipe = new RecipeBuilder().Build();
        #endregion

        #region Act
        await _recipeRepository.Create(unSavedRecipe);
        #endregion

        #region Assert
        Recipe? savedRecipe = await _recipeRepository.GetOne(unSavedRecipe.Title);
        Assert.NotNull(savedRecipe);
        Assert.Equal(unSavedRecipe.Title, savedRecipe.Title);
        #endregion
    }

    [Fact]
    public async Task CreateRecipe_NullInput_ThrowsArgumentNullException()
    {
        #region Arrange
        Recipe? nullRecipe = null;
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _recipeRepository.Create(nullRecipe!));
        #endregion
    }

    [Theory]
    [InlineData(null, "Valid Description", CuisineType.Italian, DifficultyLevel.Easy)]
    [InlineData("", "Valid Description", CuisineType.Italian, DifficultyLevel.Easy)]
    [InlineData("Valid Title", "", CuisineType.Italian, DifficultyLevel.Easy)]
    [InlineData("Valid Title", "Description too long... (too many characters)", CuisineType.Italian, DifficultyLevel.Easy)]
    public async Task CreateRecipe_InvalidInputs_ThrowsValidationException(string title, string description, CuisineType type, DifficultyLevel difficultyLevel)
    {
        #region Arrange
        Recipe unSavedRecipe = new Recipe { Title = title, Description = description, CuisineType = type, DifficultyLevel = difficultyLevel };
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _recipeRepository.Create(unSavedRecipe));
        #endregion
    }

    [Fact]
    public async Task CreateRecipe_DuplicateEntity_ThrowsArgumentException()
    {
        #region Arrange
        Recipe unSavedRecipe = new RecipeBuilder().Build();
        Recipe sameRecipe = unSavedRecipe;
        #endregion

        #region Act
        await _recipeRepository.Create(unSavedRecipe);
        #endregion

        #region Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _recipeRepository.Create(sameRecipe));
        #endregion
    }

    [Fact]
    public async Task CreateRecipe_WithRelations_EnsuresCorrectForeignKeysAndSavesRelatedEntities()
    {
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
        _dbContext.Ingredients.AddRange(ingredients);
        _dbContext.Categories.AddRange(categories);
        await _dbContext.SaveChangesAsync();

        // Create a new recipe with ingredients and categories
        Recipe unSavedRecipe = new RecipeBuilder()
                                    .WithTitle("Tomato Omelette")
                                    .WithIngredients(ingredients)
                                    .WithCategories(categories)
                                    .Build();
        #endregion

        #region Act
        // Add the recipe to the repository
        Recipe savedRecipe = await _recipeRepository.Create(unSavedRecipe);
        #endregion

        #region Assert
        // Retrieve the saved recipe from the repository, ensuring all related data is included
        Recipe? retrievedRecipe = await _recipeRepository.GetOneWithAllDetails(savedRecipe.ID);
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
        await Assert.ThrowsAsync<DbUpdateException>(() => _recipeRepository.Create(unSavedRecipe));
        #endregion
    }
}
