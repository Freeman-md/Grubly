using System;
using System.ComponentModel.DataAnnotations;
using Grubly.Data;
using Grubly.Interfaces.Services;
using Grubly.Models;
using Grubly.Tests.Unit.Builders;
using Grubly.Tests.Unit.Fixtures;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.ValueContentAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Grubly.Tests.Services;

public partial class IngredientServiceTests : IClassFixture<TestFixture>
{
    private readonly ServiceProvider _serviceProvider;

    public IngredientServiceTests(TestFixture fixture) {
        _serviceProvider = fixture.ServiceProvider;
    }

    private (IIngredientService ingredientService, GrublyContext dbContext) CreateScope() {
        var scope = _serviceProvider.CreateScope();
        var scopedServices = scope.ServiceProvider;

        var ingredientService = scopedServices.GetRequiredService<IIngredientService>();
        var dbContext = scopedServices.GetRequiredService<GrublyContext>();

        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        return (ingredientService, dbContext);
    }

    [Fact]
    public async Task CreateIngredient_ValidInput_CreatesIngredientSuccessfully() {
        var (ingredientService, dbContext) = CreateScope();

        #region Arrange
            Ingredient unSavedIngredient = new IngredientBuilder().Build();
        #endregion

        #region Act
            Ingredient savedIngredient = await ingredientService.CreateIngredient(unSavedIngredient);    
        #endregion

        #region Assert
            Assert.NotNull(savedIngredient);
            Assert.True(savedIngredient.ID > 0, "The Ingredient ID should be greater than 0 after saving to the database.");
            Assert.Equal(unSavedIngredient.Name, savedIngredient.Name);
        #endregion
    }

    [Fact]
    public async Task CreateIngredient_NullInput_ThrowsArgumentNullException() {
        var (ingredientService, dbContext) = CreateScope();

        #region Act -> Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => ingredientService.CreateIngredient(null!));
        #endregion
    }

    [Fact]
    public async Task CreateIngredient_DuplicateIngredient_ThrowsDbUpdateException() {
        var (ingredientService, dbContext) = CreateScope();

        #region Arrange
            Ingredient originalIngredient = new IngredientBuilder().Build();
            Ingredient duplicateIngredient = new Ingredient {
                Name = originalIngredient.Name, 
                Description = originalIngredient.Description
            };
        #endregion

        #region Arrange -> Act
            await ingredientService.CreateIngredient(originalIngredient);
        #endregion

        #region Assert
            await Assert.ThrowsAsync<DbUpdateException>(() => ingredientService.CreateIngredient(duplicateIngredient));
        #endregion
    }

    [Theory]
    [InlineData(null, "Valid Description")]
    [InlineData("", "Valid Description")]
    [InlineData("A very long name that exceeds the maximum allowed length for the ingredient name which is 50 chars.", "Valid Description")]
    [InlineData(" ", "Valid Description")]
    [InlineData("", " ")]
    public async Task CreateIngredient_InvalidInput_ThrowsValidationException(string name, string description) {
        var (ingredientService, dbContext) = CreateScope();

        #region Arrange
            Ingredient ingredient = new Ingredient {
                Name = name,
                Description = description
            };
        #endregion

        #region Act -> Assert
            await Assert.ThrowsAsync<ValidationException>(() => ingredientService.CreateIngredient(ingredient));
        #endregion
    }

    [Fact]
    public async Task CreateIngredient_WithExistingRecipes_CreatesWithExistingRelations() {
        var (ingredientService, dbContext) = CreateScope();

        #region Arrange
            List<Recipe> recipes = RecipeBuilder.BuildMany(2);

            await dbContext.Recipes.AddRangeAsync(recipes.ToArray());
            await dbContext.SaveChangesAsync();

            Ingredient unSavedIngredient = new IngredientBuilder().WithRecipes(recipes.ToArray()).Build();
        #endregion

        #region Act
            Ingredient savedIngredient = await ingredientService.CreateIngredient(unSavedIngredient);
        #endregion

        #region Assert
            Assert.NotNull(savedIngredient);
            Assert.True(savedIngredient.ID > 0, "The Ingredient ID should be greater than 0 after saving to the database.");
            Assert.Equal(unSavedIngredient.Name, savedIngredient.Name);

            Ingredient? retrievedIngredient = await ingredientService.GetIngredient(savedIngredient.ID);

            Assert.Equal(recipes.Count, retrievedIngredient!.Recipes.Count);

            foreach (Recipe recipe in retrievedIngredient.Recipes)
            {
                Assert.Contains(retrievedIngredient.Recipes, (r) => r.Title == recipe.Title && r.Description == recipe.Description);
            }
        #endregion
    }

    [Fact]
    public async Task CreateIngredient_WithNonExistingRecipes_ThrowsKeyNotFoundException() {
        var (ingredientService, dbContext) = CreateScope();

        #region Arrange
            List<Recipe> recipes = RecipeBuilder.BuildMany(2);

            Ingredient unSavedIngredient = new IngredientBuilder().WithRecipes(recipes.ToArray()).Build();
        #endregion

        #region Act -> Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => ingredientService.CreateIngredient(unSavedIngredient));
        #endregion
    }
}
