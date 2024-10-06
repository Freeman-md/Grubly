using System;
using System.ComponentModel.DataAnnotations;
using Grubly.Data;
using Grubly.Interfaces.Repositories;
using Grubly.Interfaces.Services;
using Grubly.Models;
using Grubly.Repositories;
using Grubly.Services;
using Grubly.Tests.Unit.Builders;
using Grubly.Tests.Unit.Fixtures;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.ValueContentAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Grubly.Tests.Services;

public partial class IngredientServiceTests
{
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<IRecipeRepository> _mockRecipeRepository;
    private readonly IngredientService _ingredientService;

    public IngredientServiceTests()
    {
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockRecipeRepository = new Mock<IRecipeRepository>();

        _ingredientService = new IngredientService(_mockIngredientRepository.Object, _mockRecipeRepository.Object);
    }

    [Fact]
    public async Task CreateIngredient_ValidInput_CreatesIngredientSuccessfully()
    {
        #region Arrange
        _mockIngredientRepository.Setup(repo => repo.Create(It.IsAny<Ingredient>()))
                                .ReturnsAsync((Ingredient ingredient) =>
                                {
                                    ingredient.ID = 1;
                                    return ingredient;
                                });
        #endregion

        #region Act
        Ingredient savedIngredient = await _ingredientService.CreateIngredient(new IngredientBuilder().Build());
        #endregion

        #region Assert
        _mockIngredientRepository.Verify(repo => repo.Create(It.IsAny<Ingredient>()), Times.Once);
        Assert.NotNull(savedIngredient);
        Assert.True(savedIngredient.ID > 0);
        #endregion
    }

    [Fact]
    public async Task CreateIngredient_NullInput_ThrowsArgumentNullException()
    {
        #region Act -> Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _ingredientService.CreateIngredient(null!));
        #endregion
    }

    [Theory]
    [InlineData(null, "Valid Description")]
    [InlineData("", "Valid Description")]
    [InlineData("A very long name that exceeds the maximum allowed length for the ingredient name which is 50 chars.", "Valid Description")]
    [InlineData(" ", "Valid Description")]
    [InlineData("", " ")]
    public async Task CreateIngredient_InvalidInput_ThrowsValidationException(string name, string description)
    {
        #region Arrange
        Ingredient ingredient = new IngredientBuilder()
                                .WithName(name)
                                .WithDescription(description)
                                .Build();
        #endregion

        #region Act -> Assert
        await Assert.ThrowsAsync<ValidationException>(() => _ingredientService.CreateIngredient(ingredient));
        #endregion
    }

    [Fact]
    public async Task CreateIngredient_WithNonExistingRecipes_ThrowsKeyNotFoundException()
    {
        #region Arrange
        List<Recipe> recipes = RecipeBuilder.BuildMany(2);
        Ingredient ingredient = new IngredientBuilder().WithRecipes(recipes.ToArray()).Build();

        _mockRecipeRepository.Setup(repo => repo.GetOne(It.IsAny<int>())).ReturnsAsync((Recipe)null); // simulate non-existing categories by returning null
        #endregion

        #region Act -> Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _ingredientService.CreateIngredient(ingredient));

        _mockRecipeRepository.Verify(repo => repo.GetOne(It.IsAny<int>()), Times.Once);
        #endregion
    }
}
