using System;
using Grubly.Controllers;
using Grubly.Interfaces.Services;
using Grubly.Models;
using Grubly.Tests.Unit.Builders;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Grubly.Tests.Controllers;

public class RecipeControllerTests
{
    private readonly Mock<IRecipeService> _mockRecipeService;
    private readonly Mock<IIngredientService> _mockIngredientService;
    private readonly Mock<ICategoryService> _mockCategoryService;

            private readonly RecipeController _recipeController;


    public RecipeControllerTests()
    {
        _mockRecipeService = new Mock<IRecipeService>();
        _mockIngredientService = new Mock<IIngredientService>();
        _mockCategoryService = new Mock<ICategoryService>();

                    _recipeController = new RecipeController(_mockRecipeService.Object, _mockIngredientService.Object, _mockCategoryService.Object);

    }

    [Fact]
    public async Task Index_ReturnsViewWithRecipeList()
    {
        #region Arrange
        List<Recipe> recipes = RecipeBuilder.BuildMany(4);

        _mockRecipeService.Setup(service => service.GetAllRecipes())
                            .ReturnsAsync(recipes);
        #endregion

        #region Act
            var result = await _recipeController.Index();
        #endregion

        #region Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IReadOnlyCollection<Recipe>>(viewResult.ViewData.Model);

            Assert.Equal(recipes.Count, model.Count);
        #endregion
    }

    [Fact]
    public async Task Index_WhenNoRecipesExist_ReturnsEmptyView() {
        #region Arrange
        _mockRecipeService.Setup(service => service.GetAllRecipes())
                            .ReturnsAsync(new List<Recipe>());
        #endregion

        #region Act
            var result = await _recipeController.Index();
        #endregion

        #region Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IReadOnlyCollection<Recipe>>(viewResult.ViewData.Model);

            Assert.Empty(model);
        #endregion
    }
}
