using System;
using Grubly.Models;
using Grubly.Tests.Unit.Builders;
using Moq;

namespace Grubly.Tests.Services;

public partial class RecipeServiceTests
{

[Fact]
    public async Task GetRecipe_ById_ValidId_ReturnsRecipe()
    {
        #region Arrange

        var recipe = new RecipeBuilder().WithId(1).Build();

        _mockRecipeRepository.Setup(repo => repo.GetOne(recipe.ID))
                      .ReturnsAsync(recipe);
        #endregion

        #region Act
        var result = await _recipeService.GetRecipe(recipe.ID);
        #endregion

        #region Assert
        Assert.NotNull(result);
        Assert.Equal(recipe.ID, result.ID);
        _mockRecipeRepository.Verify(repo => repo.GetOne(recipe.ID), Times.Once);
        #endregion
    }

    [Fact]
    public async Task GetRecipe_ById_ValidId_ReturnsRecipeWithDetails()
    {
        #region Arrange

        List<Ingredient> ingredients = IngredientBuilder.BuildMany(2);

        Recipe recipe = new RecipeBuilder().WithId(1).WithIngredients(ingredients.ToArray()).Build();

        _mockRecipeRepository.Setup(repo => repo.GetOneWithAllDetails(recipe.ID))
                      .ReturnsAsync(recipe);
        #endregion

        #region Act
        var result = await _recipeService.GetRecipeWithAllDetails(recipe.ID);
        #endregion

        #region Assert
        Assert.NotNull(result);
        Assert.Equal(recipe.ID, result.ID);
        _mockRecipeRepository.Verify(repo => repo.GetOneWithAllDetails(recipe.ID), Times.Once);
        #endregion
    }

    [Fact]
    public async Task GetRecipe_ById_InvalidId_ReturnsNull()
    {
        #region Arrange
        var invalidId = 99;
        _mockRecipeRepository.Setup(repo => repo.GetOne(invalidId))
                                 .ReturnsAsync((Recipe)null);
        #endregion

        #region Act
        var result = await _recipeService.GetRecipe(invalidId);
        #endregion

        #region Assert
        Assert.Null(result);
        _mockRecipeRepository.Verify(repo => repo.GetOne(invalidId), Times.Once);
        #endregion
    }

    [Fact]
    public async Task GetRecipeByTitle_ValidTitle_ReturnsRecipe()
    {
        #region Arrange
        var recipe = new RecipeBuilder().Build();

        _mockRecipeRepository.Setup(repo => repo.GetOneWithAllDetails(recipe.Title))
                            .ReturnsAsync((recipe));
        #endregion

        #region Act
        var result = await _recipeService.GetRecipeWithAllDetails(recipe.Title);
        #endregion

        #region Assert
        Assert.NotNull(result);
        Assert.Equal(recipe.Title, result.Title);
        _mockRecipeRepository.Verify(repo => repo.GetOneWithAllDetails(recipe.Title), Times.Once);
        #endregion
    }

    [Fact]
    public async Task GetRecipeByTitle_ValidTitle_ReturnsRecipeWithDetails()
    {
        #region Arrange
        List<Ingredient> ingredients = IngredientBuilder.BuildMany(2);

        Recipe recipe = new RecipeBuilder().WithIngredients(ingredients.ToArray()).Build();

        _mockRecipeRepository.Setup(repo => repo.GetOneWithAllDetails(recipe.Title))
                            .ReturnsAsync((recipe));
        #endregion

        #region Act
        var result = await _recipeService.GetRecipeWithAllDetails(recipe.Title);
        #endregion

        #region Assert
        Assert.NotNull(result);
        Assert.Equal(recipe.Title, result.Title);
        _mockRecipeRepository.Verify(repo => repo.GetOneWithAllDetails(recipe.Title), Times.Once);
        #endregion
    }

    [Fact]
    public async Task GetRecipeByTitle_InvalidTitle_ReturnsNull()
    {
        #region Arrange
        string invalidTitle = "Rice & Beans";

        _mockRecipeRepository.Setup(repo => repo.GetOne(invalidTitle))
                            .ReturnsAsync((Recipe)null);
        #endregion

        #region Act
        var result = await _recipeService.GetRecipe(invalidTitle);
        #endregion

        #region Assert
        Assert.Null(result);
        _mockRecipeRepository.Verify(repo => repo.GetOne(invalidTitle), Times.Once);
        #endregion
    }

    [Fact]
    public async Task GetAllRecipes_ReturnsListOfRecipes() {
        #region Arrange
            List<Recipe> recipes = RecipeBuilder.BuildMany(4);

            _mockRecipeRepository.Setup(repo => repo.GetAll())
                            .ReturnsAsync(recipes);         
        #endregion

        #region Act
            IReadOnlyCollection<Recipe> retrievedRecipes = await _recipeService.GetAllRecipes();
        #endregion

        #region Assert
            Assert.NotNull(retrievedRecipes);
            Assert.Equal(recipes.Count, retrievedRecipes.Count);

            _mockRecipeRepository.Verify(repo => repo.GetAll(), Times.Once);
        #endregion
    }

    [Fact]
    public async Task GetAllRecipes_EmptyDatabase_ReturnsEmptyList() {
        #region Arrange
            List<Recipe> recipes = new List<Recipe>();

            _mockRecipeRepository.Setup(repo => repo.GetAll())
                            .ReturnsAsync(recipes);         
        #endregion

        #region Act
            IReadOnlyCollection<Recipe> retrievedRecipes = await _recipeService.GetAllRecipes();
        #endregion

        #region Assert
            Assert.NotNull(retrievedRecipes);
            Assert.Empty(retrievedRecipes);

            _mockRecipeRepository.Verify(repo => repo.GetAll(), Times.Once);
        #endregion
    }

}
