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

    [Fact]
    public async Task Edit_Get_NoIngredientsOrCategories_ReturnsEditViewWithEmptyLists()
    {
        // Arrange
        _mockIngredientService.Setup(service => service.GetAllIngredients()).ReturnsAsync(new List<Ingredient>());
        _mockCategoryService.Setup(service => service.GetAllCategories()).ReturnsAsync(new List<Category>());
        _mockRecipeService.Setup(service => service.GetRecipeWithAllDetails(It.IsAny<int>())).ReturnsAsync(new RecipeBuilder().Build());

        // Act
        var result = await _recipeController.Edit(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<RecipeFormViewModel>(viewResult.Model);
        Assert.Empty(model.AvailableIngredients);
        Assert.Empty(model.AvailableCategories);
    }


    [Fact]
    public async Task Update_Post_ValidData_UpdatesRecipeAndRedirectsToShow()
    {
        // Arrange
        #region Arrange
        Recipe recipe = new RecipeBuilder().WithId(1).Build();
        _mockIngredientService.Setup(service => service.GetAllIngredients()).ReturnsAsync(IngredientBuilder.BuildMany(2));
        _mockCategoryService.Setup(service => service.GetAllCategories()).ReturnsAsync(CategoryBuilder.BuildMany(2));
        _mockRecipeService.Setup(service => service.GetRecipeWithAllDetails(It.IsAny<int>())).ReturnsAsync(recipe);

        var viewModel = new RecipeFormViewModel
        {
            Title = recipe.Title,
            Description = recipe.Description,
            CuisineType = recipe.CuisineType,
            DifficultyLevel = recipe.DifficultyLevel,
            SelectedIngredients = new[] { true },
            SelectedCategories = new[] { true },
            AvailableIngredients = IngredientBuilder.BuildMany(1),
            AvailableCategories = CategoryBuilder.BuildMany(1),
        };

        _mockRecipeService.Setup(service => service.UpdateRecipe(It.IsAny<Recipe>(), It.IsAny<int>()))
                          .ReturnsAsync(new Recipe { ID = 1 });
        #endregion

        // Act
        #region Act
        var result = await _recipeController.Update(viewModel, recipe.ID);
        #endregion

        // Assert
        #region Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Show", redirectResult.ActionName);

        _mockRecipeService.Verify(service => service.UpdateRecipe(It.Is<Recipe>(r =>
            r.Title == viewModel.Title &&
            r.Description == viewModel.Description &&
            r.Ingredients.Count == 1 &&
            r.Categories.Count == 1
        ), recipe.ID), Times.Once);
        #endregion
    }


    [Fact]
    public async Task Update_Post_InvalidData_ReturnsEditViewWithErrors()
    {
        // Arrange
        #region Arrange
        _mockIngredientService.Setup(service => service.GetAllIngredients()).ReturnsAsync(IngredientBuilder.BuildMany(2));
        _mockCategoryService.Setup(service => service.GetAllCategories()).ReturnsAsync(CategoryBuilder.BuildMany(2));

        var invalidViewModel = new RecipeFormViewModel
        {
            Title = "", // Invalid: empty title
            Description = "Test Description",
            CuisineType = CuisineType.Italian,
            DifficultyLevel = DifficultyLevel.Medium,
            SelectedIngredients = new bool[0], // Invalid: no ingredients selected
            SelectedCategories = new bool[0], // Invalid: no categories selected
        };

        _recipeController.ModelState.AddModelError("Title", "The Title field is required.");
        _recipeController.ModelState.AddModelError("SelectedIngredients", "Please select at least one ingredient.");
        _recipeController.ModelState.AddModelError("SelectedCategories", "Please select at least one category.");
        #endregion

        // Act
        #region Act
        var result = await _recipeController.Update(invalidViewModel, 1);
        #endregion

        // Assert
        #region Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<RecipeFormViewModel>(viewResult.Model);
        Assert.False(_recipeController.ModelState.IsValid);
        Assert.Equal(3, _recipeController.ModelState.ErrorCount);

        _mockRecipeService.Verify(service => service.UpdateRecipe(It.IsAny<Recipe>(), It.IsAny<int>()), Times.Never);
        #endregion
    }


}
