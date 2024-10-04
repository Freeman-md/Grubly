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

public partial class IngredientServiceTests : IClassFixture<TestFixture>
{
    private readonly ServiceProvider _serviceProvider;
    private readonly Mock<IIngredientRepository> _mockRepository;
    private readonly IngredientService _service;

    public IngredientServiceTests(TestFixture fixture)
    {
        _serviceProvider = fixture.ServiceProvider;

        _mockRepository = new Mock<IIngredientRepository>();
        _service = new IngredientService(_mockRepository.Object);
    }

    private (IIngredientService ingredientService, GrublyContext dbContext) CreateScope()
    {
        var scope = _serviceProvider.CreateScope();
        var scopedServices = scope.ServiceProvider;

        var ingredientService = scopedServices.GetRequiredService<IIngredientService>();
        var dbContext = scopedServices.GetRequiredService<GrublyContext>();

        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        return (ingredientService, dbContext);
    }

    [Fact]
    public async Task CreateIngredient_ValidInput_CreatesIngredientSuccessfully()
    {
        #region Arrange
        _mockRepository.Setup(repo => repo.Create(It.IsAny<Ingredient>()))
                                .ReturnsAsync((Ingredient ingredient) =>
                                {
                                    ingredient.ID = 1;
                                    return ingredient;
                                });
        #endregion

        #region Act
        Ingredient savedIngredient = await _service.CreateIngredient(new IngredientBuilder().Build());
        #endregion

        #region Assert
        _mockRepository.Verify(repo => repo.Create(It.IsAny<Ingredient>()), Times.Once);
        Assert.NotNull(savedIngredient);
        Assert.True(savedIngredient.ID > 0);
        #endregion
    }

    [Fact]
    public async Task CreateIngredient_NullInput_ThrowsArgumentNullException()
    {
        #region Act -> Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateIngredient(null!));
        #endregion
    }

    [Fact]
    public async Task CreateIngredient_DuplicateIngredient_ThrowsDbUpdateException()
    {
        #region Arrange
        Ingredient ingredient = new IngredientBuilder().WithName("Tomato").WithId(1).Build();

        _mockRepository.Setup(repo => repo.Create(ingredient))
                        .ReturnsAsync(ingredient);

        _mockRepository.Setup(repo => repo.Create(It.Is<Ingredient>(i => i.Name == "Tomato")))
                        .ThrowsAsync(new DbUpdateException());
        #endregion

        #region Act
        await _service.CreateIngredient(ingredient);

        #endregion

        #region Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => _service.CreateIngredient(ingredient));

        _mockRepository.Verify(repo => repo.Create(It.IsAny<Ingredient>()), Times.Exactly(2));

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
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateIngredient(ingredient));
        #endregion
    }

    [Fact]
    public async Task CreateIngredient_WithExistingRecipes_CreatesWithExistingRelations()
    {
        #region Arrange
        _mockRepository.Setup(repo => repo.Create(It.IsAny<Ingredient>()))
           .ReturnsAsync((Ingredient ingredient) =>
           {
               ingredient.ID = 1;
               return ingredient;
           });

        List<Recipe> recipes = RecipeBuilder.BuildMany(3);
        #endregion

        #region Act
        Ingredient savedIngredient = await _service.CreateIngredient(
            new IngredientBuilder().WithRecipes(recipes.ToArray()).Build()
        );
        #endregion

        #region Assert
        _mockRepository.Verify(repo => repo.Create(It.IsAny<Ingredient>()), Times.Once);

        Assert.NotNull(savedIngredient);
        Assert.Equal(recipes.Count, savedIngredient.Recipes.Count);

        foreach (Recipe recipe in savedIngredient.Recipes)
        {
            Assert.Contains(savedIngredient.Recipes, (r) => r.Title == recipe.Title && r.Description == recipe.Description);
        }
        #endregion
    }

    [Fact]
    public async Task CreateIngredient_WithNonExistingRecipes_ThrowsKeyNotFoundException()
    {
        #region Arrange
        List<Recipe> recipes = RecipeBuilder.BuildMany(2);
        Ingredient ingredient = new IngredientBuilder().WithRecipes(recipes.ToArray()).Build();

        _mockRepository.Setup(repo => repo.Create(It.IsAny<Ingredient>()))
               .ThrowsAsync(new KeyNotFoundException("Recipe not found"));
        #endregion

        #region Act -> Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateIngredient(ingredient));
        #endregion
    }
}
