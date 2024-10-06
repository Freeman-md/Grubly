using System;
using Grubly.Models;
using Grubly.Tests.Unit.Builders;
using Moq;
using NuGet.Packaging.Signing;

namespace Grubly.Tests.Services;

public partial class IngredientServiceTests
{
    [Fact]
    public async Task GetIngredient_ById_ValidId_ReturnsIngredient()
    {
        #region Arrange

        var ingredient = new IngredientBuilder().WithId(1).Build();

        _mockIngredientRepository.Setup(repo => repo.GetOne(ingredient.ID))
                      .ReturnsAsync(ingredient);
        #endregion

        #region Act
        var result = await _ingredientService.GetIngredient(ingredient.ID);
        #endregion

        #region Assert
        Assert.NotNull(result);
        Assert.Equal(ingredient.ID, result.ID);
        _mockIngredientRepository.Verify(repo => repo.GetOne(ingredient.ID), Times.Once);
        #endregion
    }

    [Fact]
    public async Task GetIngredient_ById_ValidId_ReturnsIngredientWithDetails()
    {
        #region Arrange

        List<Recipe> recipes = RecipeBuilder.BuildMany(2);

        Ingredient ingredient = new IngredientBuilder().WithId(1).WithRecipes(recipes.ToArray()).Build();

        _mockIngredientRepository.Setup(repo => repo.GetOneWithAllDetails(ingredient.ID))
                      .ReturnsAsync(ingredient);
        #endregion

        #region Act
        var result = await _ingredientService.GetIngredientWithAllDetails(ingredient.ID);
        #endregion

        #region Assert
        Assert.NotNull(result);
        Assert.Equal(ingredient.ID, result.ID);
        _mockIngredientRepository.Verify(repo => repo.GetOneWithAllDetails(ingredient.ID), Times.Once);
        #endregion
    }

    [Fact]
    public async Task GetIngredient_ById_InvalidId_ReturnsNull()
    {
        #region Arrange
        var invalidId = 99;
        _mockIngredientRepository.Setup(repo => repo.GetOne(invalidId))
                                 .ReturnsAsync((Ingredient)null);
        #endregion

        #region Act
        var result = await _ingredientService.GetIngredient(invalidId);
        #endregion

        #region Assert
        Assert.Null(result);
        _mockIngredientRepository.Verify(repo => repo.GetOne(invalidId), Times.Once);
        #endregion
    }

    [Fact]
    public async Task GetIngredientByName_ValidName_ReturnsIngredient()
    {
        #region Arrange
        var ingredient = new IngredientBuilder().Build();

        _mockIngredientRepository.Setup(repo => repo.GetOneWithAllDetails(ingredient.Name))
                            .ReturnsAsync((ingredient));
        #endregion

        #region Act
        var result = await _ingredientService.GetIngredientWithAllDetails(ingredient.Name);
        #endregion

        #region Assert
        Assert.NotNull(result);
        Assert.Equal(ingredient.Name, result.Name);
        _mockIngredientRepository.Verify(repo => repo.GetOneWithAllDetails(ingredient.Name), Times.Once);
        #endregion
    }

    [Fact]
    public async Task GetIngredientByName_ValidName_ReturnsIngredientWithDetails()
    {
        #region Arrange
        List<Recipe> recipes = RecipeBuilder.BuildMany(2);

        Ingredient ingredient = new IngredientBuilder().WithRecipes(recipes.ToArray()).Build();

        _mockIngredientRepository.Setup(repo => repo.GetOneWithAllDetails(ingredient.Name))
                            .ReturnsAsync((ingredient));
        #endregion

        #region Act
        var result = await _ingredientService.GetIngredientWithAllDetails(ingredient.Name);
        #endregion

        #region Assert
        Assert.NotNull(result);
        Assert.Equal(ingredient.Name, result.Name);
        _mockIngredientRepository.Verify(repo => repo.GetOneWithAllDetails(ingredient.Name), Times.Once);
        #endregion
    }

    [Fact]
    public async Task GetIngredientByName_InvalidName_ReturnsNull()
    {
        #region Arrange
        string invalidName = "Tomato & Garlic";

        _mockIngredientRepository.Setup(repo => repo.GetOne(invalidName))
                            .ReturnsAsync((Ingredient)null);
        #endregion

        #region Act
        var result = await _ingredientService.GetIngredient(invalidName);
        #endregion

        #region Assert
        Assert.Null(result);
        _mockIngredientRepository.Verify(repo => repo.GetOne(invalidName), Times.Once);
        #endregion
    }

    [Fact]
    public async Task GetAllIngredients_ReturnsListOfIngredients() {
        #region Arrange
            List<Ingredient> ingredients = IngredientBuilder.BuildMany(4);

            _mockIngredientRepository.Setup(repo => repo.GetAll())
                            .ReturnsAsync(ingredients);         
        #endregion

        #region Act
            IReadOnlyCollection<Ingredient> retrievedIngredients = await _ingredientService.GetAllIngredients();
        #endregion

        #region Assert
            Assert.NotNull(retrievedIngredients);
            Assert.Equal(ingredients.Count, retrievedIngredients.Count);

            _mockIngredientRepository.Verify(repo => repo.GetAll(), Times.Once);
        #endregion
    }

    [Fact]
    public async Task GetAllIngredients_EmptyDatabase_ReturnsEmptyList() {
        #region Arrange
            List<Ingredient> ingredients = new List<Ingredient>();

            _mockIngredientRepository.Setup(repo => repo.GetAll())
                            .ReturnsAsync(ingredients);         
        #endregion

        #region Act
            IReadOnlyCollection<Ingredient> retrievedIngredients = await _ingredientService.GetAllIngredients();
        #endregion

        #region Assert
            Assert.NotNull(retrievedIngredients);
            Assert.Empty(retrievedIngredients);

            _mockIngredientRepository.Verify(repo => repo.GetAll(), Times.Once);
        #endregion
    }


}
