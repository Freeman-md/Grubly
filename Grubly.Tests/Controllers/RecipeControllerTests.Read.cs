using System;
using Grubly.Controllers;
using Grubly.Interfaces.Services;
using Grubly.Models;
using Grubly.Tests.Unit.Builders;
using Grubly.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Grubly.Tests.Controllers;

/******
Tests for the Index and Show Actions
********************************************************/

public partial class RecipeControllerTests
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
    public async Task Index_ReturnsViewWithExpectedViewModel()
    {
        #region Act
        var result = await _recipeController.Index();
        #endregion

        #region Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsAssignableFrom<RecipeIndexViewModel>(viewResult.ViewData.Model);
        #endregion
    }

    [Fact]
    public async Task Index_ReturnsRecipeList_WithCorrectCount()
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
        var model = Assert.IsAssignableFrom<RecipeIndexViewModel>(viewResult.ViewData.Model);

        Assert.Equal(recipes.Count, model.Recipes.Count);
        #endregion
    }

    [Fact]
    public async Task Index_ReturnsCategoryList_WithCorrectCount()
    {
        #region Arrange
        List<Category> recipes = CategoryBuilder.BuildMany(4);

        _mockCategoryService.Setup(service => service.GetAllCategories())
                            .ReturnsAsync(recipes);
        #endregion

        #region Act
        var result = await _recipeController.Index();
        #endregion

        #region Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<RecipeIndexViewModel>(viewResult.ViewData.Model);

        Assert.Equal(recipes.Count, model.Categories.Count);
        #endregion
    }

    [Fact]
    public async Task Index_ReturnsIngredientList_WithCorrectCount()
    {
        #region Arrange
        List<Ingredient> recipes = IngredientBuilder.BuildMany(4);

        _mockIngredientService.Setup(service => service.GetAllIngredients())
                            .ReturnsAsync(recipes);
        #endregion

        #region Act
        var result = await _recipeController.Index();
        #endregion

        #region Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<RecipeIndexViewModel>(viewResult.ViewData.Model);

        Assert.Equal(recipes.Count, model.Ingredients.Count);
        #endregion
    }

    [Fact]
    public async Task Index_ReturnsCuisineTypesList_WithAllEnumValues()
    {
        #region Act
        var result = await _recipeController.Index();
        #endregion

        #region Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<RecipeIndexViewModel>(viewResult.ViewData.Model);

        var expectedCuisineTypes = Enum.GetValues(typeof(CuisineType)).Cast<CuisineType>();

        Assert.All(expectedCuisineTypes, cuisineType =>
            Assert.Contains(cuisineType, model.CuisineTypes));
        #endregion
    }

    [Fact]
    public async Task Index_ReturnsDifficultyLevelsList_WithAllEnumValues()
    {
        #region Act
        var result = await _recipeController.Index();
        #endregion

        #region Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<RecipeIndexViewModel>(viewResult.ViewData.Model);

        var expectedDifficultyLevels = Enum.GetValues(typeof(DifficultyLevel)).Cast<DifficultyLevel>();

        Assert.All(expectedDifficultyLevels, difficultyLevel =>
            Assert.Contains(difficultyLevel, model.DifficultyLevels));
        #endregion
    }

    [Fact]
    public async Task Index_WhenNoDataExists_ReturnsEmptyCollections()
    {
        #region Arrange
        _mockRecipeService.Setup(service => service.GetAllRecipes())
                            .ReturnsAsync(new List<Recipe>());
        _mockCategoryService.Setup(service => service.GetAllCategories())
        .ReturnsAsync(new List<Category>());
        _mockIngredientService.Setup(service => service.GetAllIngredients())
        .ReturnsAsync(new List<Ingredient>());
        #endregion

        #region Act
        var result = await _recipeController.Index();
        #endregion

        #region Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<RecipeIndexViewModel>(viewResult.ViewData.Model);

        Assert.Empty(model.Recipes);
        Assert.Empty(model.Ingredients);
        Assert.Empty(model.Categories);
        #endregion
    }

    [Fact]
    public async Task Show_ValidId_ReturnsRecipeWithAllDetails()
    {
        #region Act
        IReadOnlyCollection<Ingredient> ingredients = IngredientBuilder.BuildMany(2);
        IReadOnlyCollection<Category> categories = CategoryBuilder.BuildMany(2);
        Recipe recipe = new RecipeBuilder()
                            .WithId(1)
                            .WithIngredients(ingredients.ToArray())
                            .WithCategories(categories.ToArray())
                            .Build();

        _mockRecipeService.Setup(service => service.GetRecipeWithAllDetails(It.Is<int>(id => id == recipe.ID))).ReturnsAsync(recipe);
        #endregion

        #region Act
        var result = await _recipeController.Show(recipe.ID);
        #endregion

        #region Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<Recipe>(viewResult.ViewData.Model);

        Assert.Equal(recipe.ID, model.ID);
        Assert.Equal(recipe.Title, model.Title);
        Assert.Equal(recipe.Description, model.Description);
        Assert.Equal(recipe.CuisineType, model.CuisineType);
        Assert.Equal(recipe.DifficultyLevel, model.DifficultyLevel);

        foreach (var ingredient in recipe.Ingredients)
        {
            Assert.Contains(model.Ingredients, i => i.Name == ingredient.Name);
        }

        foreach (var category in recipe.Categories)
        {
            Assert.Contains(model.Categories, i => i.Name == category.Name);
        }
        #endregion
    }

    [Fact]
    public async Task Show_InvalidId_ReturnsNotFound() {
        #region Act
        _mockRecipeService.Setup(service => service.GetRecipeWithAllDetails(It.IsAny<int>())).ReturnsAsync((Recipe)null!);
        #endregion

        #region Act
        var result = await _recipeController.Show(-1);
        #endregion

        #region Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("NotFound", viewResult.ViewName);
        #endregion
    }
}
