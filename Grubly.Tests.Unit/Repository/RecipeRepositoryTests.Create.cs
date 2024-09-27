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
        Recipe newRecipe = new RecipeBuilder().Build();
        #endregion

        #region Act
        await _recipeRepository.Create(newRecipe);
        #endregion

        #region Assert
        Recipe? savedRecipe = await _recipeRepository.GetOne(newRecipe.Title);
        Assert.NotNull(savedRecipe);
        Assert.Equal(newRecipe.Title, savedRecipe.Title);
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
        Recipe recipe = new Recipe { Title = title, Description = description, CuisineType = type, DifficultyLevel = difficultyLevel };
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _recipeRepository.Create(recipe));
        #endregion
    }

    [Fact]
    public async Task CreateRecipe_DuplicateEntity_ThrowsArgumentException()
    {
        #region Arrange
        Recipe newRecipe = new RecipeBuilder().Build();
        Recipe sameRecipe = newRecipe;
        #endregion

        #region Act
        await _recipeRepository.Create(newRecipe);
        #endregion

        #region Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _recipeRepository.Create(sameRecipe));
        #endregion
    }

    [Fact]
    public async Task CreateRecipe_DatabaseIntegrity_EnsuresCorrectForeignKeys()
    {
        #region Arrange
        var ingredient1 = new Ingredient { Name = "Tomato", Description = "Fresh red tomatoes" };
        var ingredient2 = new Ingredient { Name = "Garlic", Description = "Fresh garlic cloves" };
        var category = new Category { Name = "Breakfast" };

        _dbContext.Ingredients.Add(ingredient1);
        _dbContext.Ingredients.Add(ingredient2);
        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        Recipe recipe = new RecipeBuilder()
        .WithTitle("Tomato Omelette")
        .WithIngredients(ingredient1, ingredient2)
        .WithCategories(category)
        .Build();
        #endregion

        #region Act
        Recipe newRecipe = await _recipeRepository.Create(recipe);
        #endregion

        #region Assert
        Recipe? savedRecipe = await _recipeRepository.GetOneWithAllDetails(newRecipe.ID);
        Assert.NotNull(savedRecipe);
        Assert.Contains(savedRecipe.Ingredients!, i => i.Name == "Tomato");
        Assert.Contains(savedRecipe.Categories!, c => c.Name == "Breakfast");
        #endregion
    }

    [Fact]
    public async Task CreateRecipe_InvalidForeignKey_ThrowsForeignKeyViolationException()
    {
        #region Arrange
        var invalidIngredient = new Ingredient { ID = 999, Name = "Non-Existent Ingredient" }; // Invalid ID
        var invalidCategory = new Category { ID = 999, Name = "Non-Existent Category" }; // Invalid ID

        // Build the recipe with these non-existent entities
        Recipe newRecipe = new RecipeBuilder()
            .WithTitle("Invalid Recipe")
            .WithIngredients(invalidIngredient)  // Invalid reference
            .WithCategories(invalidCategory)     // Invalid reference
            .Build();
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => _recipeRepository.Create(newRecipe));
        #endregion
    }
}
