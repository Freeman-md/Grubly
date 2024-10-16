using System;
using System.ComponentModel.DataAnnotations;
using Grubly.Models;
using Grubly.Tests.Unit.Builders;
using Grubly.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Grubly.Tests.Controllers;

public partial class RecipeControllerTests
{
    [Fact]
    public async void Create_Get_ReturnsCreateView()
    {
        #region Arrange
        List<Ingredient> ingredients = IngredientBuilder.BuildMany(2);
        List<Category> categories = CategoryBuilder.BuildMany(2);

        _mockIngredientService.Setup(service => service.GetAllIngredients()).ReturnsAsync(ingredients);
        _mockCategoryService.Setup(service => service.GetAllCategories()).ReturnsAsync(categories);
        #endregion

        #region Act
        var result = await _recipeController.Create();
        #endregion

        #region Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<RecipeFormViewModel>(viewResult.Model);
        Assert.Equal(ingredients.Count, model.AvailableIngredients.Count);
        Assert.Equal(categories.Count, model.AvailableCategories.Count);

        #endregion
    }

    [Fact]
    public async Task Create_Get_NoIngredientsOrCategories_ReturnsCreateViewWithEmptyLists()
    {
        // Arrange
        _mockIngredientService.Setup(service => service.GetAllIngredients()).ReturnsAsync(new List<Ingredient>());
        _mockCategoryService.Setup(service => service.GetAllCategories()).ReturnsAsync(new List<Category>());

        // Act
        var result = await _recipeController.Create();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<RecipeFormViewModel>(viewResult.Model);
        Assert.Empty(model.AvailableIngredients);
        Assert.Empty(model.AvailableCategories);
    }


    [Fact]
    public async Task Create_Post_ValidData_CreatesRecipeAndRedirectsToIndex()
    {
        #region Arrange
        _mockIngredientService.Setup(service => service.GetAllIngredients()).ReturnsAsync(IngredientBuilder.BuildMany(2));
        _mockCategoryService.Setup(service => service.GetAllCategories()).ReturnsAsync(CategoryBuilder.BuildMany(2));

        var viewModel = new RecipeFormViewModel
        {
            Title = "Test Recipe",
            Description = "Test Description",
            CuisineType = CuisineType.Italian,
            DifficultyLevel = DifficultyLevel.Medium,
            SelectedIngredients = new[] { true },
            SelectedCategories = new[] { true },
            AvailableIngredients = IngredientBuilder.BuildMany(1),
            AvailableCategories = CategoryBuilder.BuildMany(1),
        };

        _mockRecipeService.Setup(service => service.CreateRecipe(It.IsAny<Recipe>()))
                        .ReturnsAsync(new Recipe { ID = 1 });
        #endregion

        #region Act
        var result = await _recipeController.Create(viewModel);
        #endregion

        #region Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);

        _mockRecipeService.Verify(service => service.CreateRecipe(It.Is<Recipe>(r =>
            r.Title == viewModel.Title &&
            r.Description == viewModel.Description &&
            r.Ingredients.Count == 1 &&
            r.Categories.Count == 1)), Times.Once);
        #endregion
    }

    [Fact]
    public async Task Create_Post_InvalidData_ReturnsCreateViewWithErrors()
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

        #region Act
        var result = await _recipeController.Create(invalidViewModel);
        #endregion

        #region Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<RecipeFormViewModel>(viewResult.Model);
        Assert.False(_recipeController.ModelState.IsValid);
        Assert.Equal(3, _recipeController.ModelState.ErrorCount);

        _mockRecipeService.Verify(service => service.CreateRecipe(It.IsAny<Recipe>()), Times.Never);
        #endregion
    }


}
