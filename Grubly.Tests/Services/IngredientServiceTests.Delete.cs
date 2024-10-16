using Grubly.Models;
using Grubly.Tests.Unit.Builders;
using Moq;

namespace Grubly.Tests.Services;

public partial class IngredientServiceTests
{
    [Fact]
    public async Task DeleteIngredient_CallsRepositoryDeleteMethodSuccessfully()
    {
        #region Arrange
        Ingredient ingredient = new IngredientBuilder().Build();

        _mockIngredientRepository.Setup(repo => repo.GetOne(It.IsAny<int>())).ReturnsAsync(ingredient);
        #endregion


        #region Act
        await _ingredientService.DeleteIngredient(ingredient.ID);
        #endregion

        #region Assert
        _mockIngredientRepository.Verify(repo => repo.Delete(ingredient.ID), Times.Once,
        "The Delete method should be called exactly once with the correct ingredient ID.");
        #endregion
    }

    [Fact]
    public async Task DeleteIngredient_ThatDoesNotExist_ThrowsKeyNotFoundException()
    {
        #region Arrange
        _mockIngredientRepository.Setup(repo => repo.GetOne(It.IsAny<int>())).ReturnsAsync((Ingredient)null);
        #endregion

        #region Act -> Assert 
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _ingredientService.DeleteIngredient(2));

        _mockIngredientRepository.Verify(repo => repo.GetOne(It.IsAny<int>()), Times.Once);
        #endregion
    }


}
