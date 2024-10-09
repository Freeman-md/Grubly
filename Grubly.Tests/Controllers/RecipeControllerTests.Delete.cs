using System;
using Grubly.Models;
using Grubly.Tests.Unit.Builders;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Grubly.Tests.Controllers;

public partial class RecipeControllerTests
{
    [Fact]
    public async Task Delete_ValidId_DeletesRecipeAndRedirectsToIndex()
    {
        #region Arrange
        int validId = 1;

        _mockRecipeService.Setup(service => service.DeleteRecipe(It.Is<int>(id => validId == id))).Returns(Task.CompletedTask);
        #endregion

        #region Act
        var result = await _recipeController.Delete(validId);
        #endregion

        #region Assert
        _mockRecipeService.Verify(service => service.DeleteRecipe(It.Is<int>(id => validId == id)), Times.Once);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        #endregion
    }

    [Fact]
    public async Task Delete_InvalidId_ReturnsNotFound()
    {
        #region Arrange
        int invalidRecipeId = 999;

        _mockRecipeService.Setup(service => service.DeleteRecipe(It.Is<int>(id => id == invalidRecipeId)))
                          .ThrowsAsync(new KeyNotFoundException());
        #endregion

        #region Act
        var result = await _recipeController.Delete(invalidRecipeId);
        #endregion

        #region Assert
        _mockRecipeService.Verify(service => service.DeleteRecipe(It.Is<int>(id => id == invalidRecipeId)), Times.Once);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("NotFound", viewResult.ViewName);
        #endregion
    }

}
