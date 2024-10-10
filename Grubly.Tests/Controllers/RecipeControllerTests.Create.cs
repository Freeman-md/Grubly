using System;
using System.ComponentModel.DataAnnotations;
using Grubly.Models;
using Grubly.Tests.Unit.Builders;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Grubly.Tests.Controllers;

public partial class RecipeControllerTests
{
    [Fact]
    public void Create_Get_ReturnsCreateView()
    {
        #region Act
        var result = _recipeController.Create();
        #endregion

        #region Assert
        Assert.IsType<ViewResult>(result);
        #endregion
    }

    [Fact]
    public async Task Create_Post_ValidData_CreatesRecipeAndRedirectsToIndex()
    {
        #region Arrange
        Recipe recipe = new RecipeBuilder().Build();

        _mockRecipeService.Setup(service => service.CreateRecipe(It.IsAny<Recipe>()))
                        .ReturnsAsync(recipe);
        #endregion

        #region Act
        var result = await _recipeController.Create(recipe);
        #endregion

        #region Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);

        _mockRecipeService.Verify(service => service.CreateRecipe(It.Is<Recipe>(r => r.Title == recipe.Title && r.Description == recipe.Description)), Times.Once);

        #endregion
    }

    [Fact]
    public async Task Create_Post_InvalidData_ReturnsCreateViewWithErrors()
    {
        // Arrange
        var invalidRecipe = new RecipeBuilder()
            .WithTitle("")
            .Build();

        _recipeController.ModelState.AddModelError("Title", "The Title field is required."); // Simulate model validation failure

        // Act
        var result = await _recipeController.Create(invalidRecipe);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);

        // Ensure the ModelState contains validation errors
        Assert.False(_recipeController.ModelState.IsValid);
        Assert.Contains("Title", _recipeController.ModelState.Keys);

        // Verify that CreateRecipe was never called due to validation failure
        _mockRecipeService.Verify(service => service.CreateRecipe(It.IsAny<Recipe>()), Times.Never);
    }

}
