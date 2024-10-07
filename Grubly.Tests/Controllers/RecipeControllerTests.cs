using System;
using Grubly.Interfaces.Services;
using Moq;

namespace Grubly.Tests.Controllers;

public class RecipeControllerTests
{
    private readonly Mock<IRecipeService> _recipeService;
    private readonly Mock<IIngredientService> _ingredientService;
    private readonly Mock<ICategoryService> _categoryService;

    public RecipeControllerTests() {
        _recipeService = new Mock<IRecipeService>();
        _ingredientService = new Mock<IIngredientService>();
        _categoryService = new Mock<ICategoryService>();
    }
}
