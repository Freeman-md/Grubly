using System;
using Grubly.Models;
using Grubly.Tests.Unit.Builders;
using Grubly.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Grubly.Tests.Controllers;

public partial class RecipeControllerTests
{

    [Fact]
    public async Task EditRecipe_WithValidID_ReturnsEditView()
    {
        #region Arrange
        List<Ingredient> ingredients = IngredientBuilder.BuildMany(2);
        List<Category> categories = CategoryBuilder.BuildMany(2);

        Recipe recipe = new RecipeBuilder()
                            .WithId(1)
                            .WithIngredients(ingredients.ToArray())
                            .WithCategories(categories.ToArray())
                            .Build();

        _mockIngredientService.Setup(service => service.GetAllIngredients())
                                  .ReturnsAsync(ingredients);
        _mockCategoryService.Setup(service => service.GetAllCategories())
                            .ReturnsAsync(categories);
        _mockRecipeService.Setup(service => service.GetRecipeWithAllDetails(It.Is<int>(id => recipe.ID == id)))
                            .ReturnsAsync(recipe);
        #endregion

        #region Act
        var result = await _recipeController.Edit(recipe.ID);
        #endregion

        #region Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<RecipeFormViewModel>(viewResult.ViewData.Model);

        Assert.Equal(model.Title, recipe.Title);
        #endregion
    }

    [Fact]
    public async Task EditRecipe_InValidID_ReturnsNotFoundView()
    {
        #region Arrange
        _mockRecipeService.Setup(service => service.GetRecipe(It.IsAny<int>()))
                            .ReturnsAsync((Recipe)null!);
        #endregion

        #region Act
        var result = await _recipeController.Edit(1);
        #endregion

        #region Assert
        #region Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("NotFound", viewResult.ViewName);
        #endregion
        #endregion
    }

}
