using System;
using Grubly.Models;
using Grubly.Tests.Unit.Builders;
using Moq;

namespace Grubly.Tests.Services;

public partial class IngredientServiceTests
{
    [Fact]
    public async Task GetIngredient_ById_ValidId_ReturnsIngredientWithDetails()
    {
        #region Arrange

        var ingredientId = 1;
        var ingredient = new Ingredient { ID = ingredientId, Name = "Tomato" };

        _mockRepository.Setup(repo => repo.GetOneWithAllDetails(ingredientId))
                      .ReturnsAsync(ingredient);
        #endregion

        #region Act
        var result = await _service.GetIngredient(ingredientId);
        #endregion

        #region Assert
        Assert.NotNull(result);
        Assert.Equal(ingredientId, result.ID);
        _mockRepository.Verify(repo => repo.GetOneWithAllDetails(ingredientId), Times.Once);
        #endregion
    }

    [Fact]
    public async Task GetIngredient_ById_InvalidId_ReturnsNull()
    {
        #region Arrange
        var invalidId = 99;
        _mockRepository.Setup(repo => repo.GetOneWithAllDetails(invalidId))
                                 .ReturnsAsync((Ingredient)null);
        #endregion

        #region Act
        var result = await _service.GetIngredient(invalidId);
        #endregion

        #region Assert
        Assert.Null(result);
        _mockRepository.Verify(repo => repo.GetOneWithAllDetails(invalidId), Times.Once);
        #endregion
    }

    [Fact]
    public async Task GetIngredientByName_ValidName_ReturnsIngredientWithDetails()
    {
        #region Arrange
        var ingredientName = "Tomato";
        var ingredient = new Ingredient { Name = ingredientName };

        _mockRepository.Setup(repo => repo.GetOneWithAllDetails(ingredientName))
                            .ReturnsAsync((ingredient));
        #endregion

        #region Act
        var result = await _service.GetIngredient(ingredientName);
        #endregion

        #region Assert
        Assert.NotNull(result);
        Assert.Equal(ingredientName, result.Name);
        _mockRepository.Verify(repo => repo.GetOneWithAllDetails(ingredientName), Times.Once);
        #endregion
    }

    [Fact]
    public async Task GetIngredientByName_InvalidName_ReturnsNull()
    {
        #region Arrange
        string invalidName = "Tomato & Garlic";

        _mockRepository.Setup(repo => repo.GetOneWithAllDetails(invalidName))
                            .ReturnsAsync((Ingredient)null);
        #endregion

        #region Act
        var result = await _service.GetIngredient(invalidName);
        #endregion

        #region Assert
        Assert.Null(result);
        _mockRepository.Verify(repo => repo.GetOneWithAllDetails(invalidName), Times.Once);
        #endregion
    }

    [Fact]
    public async Task GetAllIngredients_ReturnsListOfIngredients() {
        #region Arrange
            List<Ingredient> ingredients = IngredientBuilder.BuildMany(4);

            _mockRepository.Setup(repo => repo.GetAll())
                            .ReturnsAsync(ingredients);         
        #endregion

        #region Act
            IReadOnlyCollection<Ingredient> retrievedIngredients = await _service.GetAllIngredients();
        #endregion

        #region Assert
            Assert.NotNull(retrievedIngredients);
            Assert.Equal(ingredients.Count, retrievedIngredients.Count);

            _mockRepository.Verify(repo => repo.GetAll(), Times.Once);
        #endregion
    }

    [Fact]
    public async Task GetAllIngredients_EmptyDatabase_ReturnsEmptyList() {
        #region Arrange
            List<Ingredient> ingredients = new List<Ingredient>();

            _mockRepository.Setup(repo => repo.GetAll())
                            .ReturnsAsync(ingredients);         
        #endregion

        #region Act
            IReadOnlyCollection<Ingredient> retrievedIngredients = await _service.GetAllIngredients();
        #endregion

        #region Assert
            Assert.NotNull(retrievedIngredients);
            Assert.Empty(retrievedIngredients);

            _mockRepository.Verify(repo => repo.GetAll(), Times.Once);
        #endregion
    }


}
