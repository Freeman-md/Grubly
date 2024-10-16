using System;
using System.ComponentModel.DataAnnotations;
using Grubly.Interfaces.Repositories;
using Grubly.Models;
using Grubly.Services;
using Grubly.Tests.Unit.Builders;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Grubly.Tests.Services;

public partial class RecipeServiceTests
{
    private readonly Mock<IRecipeRepository> _mockRecipeRepository;
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly RecipeService _recipeService;

    public RecipeServiceTests()
    {
        _mockRecipeRepository = new Mock<IRecipeRepository>();
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();

        _recipeService = new RecipeService(_mockRecipeRepository.Object, _mockIngredientRepository.Object, _mockCategoryRepository.Object);
    }

    [Fact]
    public async void CreateRecipe_ValidInput_CreatesRecipeSuccessfully()
    {
        #region Arrange
        Recipe recipe = new RecipeBuilder().Build();

        _mockRecipeRepository.Setup(repo => repo.Create(It.IsAny<Recipe>()))
                            .ReturnsAsync(recipe);
        #endregion

        #region Act
        var result = await _recipeService.CreateRecipe(recipe);
        #endregion

        #region Assert
        Assert.NotNull(result);
        Assert.Equal(recipe.Title, result.Title);

        _mockRecipeRepository.Verify(repo => repo.Create(It.IsAny<Recipe>()), Times.Once);
        #endregion
    }

    [Fact]
    public async void CreateRecipe_NullInput_ThrowsArgumentNullException()
    {
        #region Act -> Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _recipeService.CreateRecipe(null!));
        #endregion
    }

    [Theory]
    [InlineData(null, "Valid Description", CuisineType.Italian, DifficultyLevel.Easy, "Step 1", "https://example.com/image.jpg")] // Null Title
    [InlineData("", "Valid Description", CuisineType.Chinese, DifficultyLevel.Medium, "Step 1", "https://example.com/image.jpg")] // Empty Title
    [InlineData("A very long title that exceeds the maximum allowed length for the recipe title which is 50 chars.", "Valid Description", CuisineType.Mexican, DifficultyLevel.Hard, "Step 1", "https://example.com/image.jpg")] // Exceeding max length Title
    [InlineData(" ", "Valid Description", CuisineType.Italian, DifficultyLevel.Easy, "Step 1", "https://example.com/image.jpg")] // Whitespace Title
    [InlineData("Valid Title", "", CuisineType.Italian, DifficultyLevel.Easy, "Step 1", "https://example.com/image.jpg")] // Empty Description
    [InlineData("Valid Title", null, CuisineType.Italian, DifficultyLevel.Easy, "Step 1", "https://example.com/image.jpg")] // Null Description
    [InlineData("Valid Title", "A very long description that exceeds the maximum allowed length for the recipe description, which should trigger validation", CuisineType.Italian, DifficultyLevel.Easy, "Step 1", "https://example.com/image.jpg")] // Long Description
    [InlineData("Valid Title", "Valid Description", (CuisineType)(-1), DifficultyLevel.Easy, "Step 1", "https://example.com/image.jpg")] // Null CuisineType
    [InlineData("Valid Title", "Valid Description", CuisineType.Italian, (DifficultyLevel)(-1), "Step 1", "https://example.com/image.jpg")] // Null DifficultyLevel
    public async Task CreateRecipe_InvalidInput_ThrowsValidationException(string title, string description, CuisineType cuisineType, DifficultyLevel difficultyLevel, string instructions, string imageUrl)
    {
        #region Arrange
        Recipe recipe = new RecipeBuilder()
                            .WithTitle(title)
                            .WithDescription(description)
                            .WithCuisineType(cuisineType)
                            .WithDifficultyLevel(difficultyLevel)
                            .WithInstructions(instructions)
                            .WithImageUrl(imageUrl)
                            .Build();
        #endregion

        #region Act -> Assert
        await Assert.ThrowsAsync<ValidationException>(() => _recipeService.CreateRecipe(recipe));
        #endregion
    }

    [Fact]
    public async Task CreateRecipe_WithNonExistingIngredients_ThrowsKeyNotFoundException()
    {
        #region Arrange
        List<Ingredient> ingredients = IngredientBuilder.BuildMany(2);
        Recipe recipe = new RecipeBuilder()
                                .WithIngredients(ingredients.ToArray())
                                .Build();

        _mockIngredientRepository.Setup(repo => repo.GetOne(It.IsAny<int>())).ReturnsAsync((Ingredient)null); // simulate non-existing ingredient by returning null
        #endregion

        #region Act -> Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _recipeService.CreateRecipe(recipe));

        _mockIngredientRepository.Verify(repo => repo.GetOne(It.IsAny<int>()), Times.Once);
        #endregion
    }

    [Fact]
    public async Task CreateRecipe_WithNonExistingCategories_ThrowsKeyNotFoundException()
    {
        #region Arrange
        List<Category> categories = CategoryBuilder.BuildMany(2);
        Recipe recipe = new RecipeBuilder()
                                .WithCategories(categories.ToArray())
                                .Build();

        _mockCategoryRepository.Setup(repo => repo.GetOne(It.IsAny<int>())).ReturnsAsync((Category)null); // simulate non-existing categories by returning null
        #endregion

        #region Act -> Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _recipeService.CreateRecipe(recipe));

        _mockCategoryRepository.Verify(repo => repo.GetOne(It.IsAny<int>()), Times.Once);
        #endregion
    }

}
