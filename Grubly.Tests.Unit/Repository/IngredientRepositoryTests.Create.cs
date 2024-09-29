using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Grubly.Data;
using Grubly.Models;
using Grubly.Repositories;
using Grubly.Tests.Unit.Builders;
using Grubly.Tests.Unit.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace Grubly.Tests.Unit.Repository;

public partial class IngredientRepositoryTests : IClassFixture<TestFixtureBase>
{
    private readonly GrublyContext _dbContext;
    private readonly IngredientRepository _ingredientRepository;

    public IngredientRepositoryTests(TestFixtureBase fixture) {
        _dbContext = fixture.DbContext;
        _ingredientRepository = new IngredientRepository(_dbContext);

        fixture.ResetDatabase().Wait();
    }

    [Fact]
    public async Task CreateIngredient_ValidInput_AddsIngredientToDatabase()
    {
        #region Arrange
        Ingredient unSavedIngredient = new IngredientBuilder().Build();
        #endregion

        #region Act
        await _ingredientRepository.Create(unSavedIngredient);
        #endregion

        #region Assert
        Ingredient? savedIngredient = await _ingredientRepository.GetOne(unSavedIngredient.Name);
        Assert.NotNull(savedIngredient);
        Assert.Equal(unSavedIngredient.Name, savedIngredient.Name);
        #endregion
    }

    [Fact]
    public async Task CreateIngredient_NullInput_ThrowsArgumentNullException()
    {
        #region Arrange
        Ingredient? nullIngredient = null;
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _ingredientRepository.Create(nullIngredient!));
        #endregion
    }

    [Theory]
    [InlineData(null, "Valid Description")]
    [InlineData("", "Valid Description")]
    public async Task CreateIngredient_InvalidInputs_ThrowsValidationException(string title, string description)
    {
        #region Arrange
        Ingredient unSavedIngredient = new Ingredient { Name = title, Description = description};
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _ingredientRepository.Create(unSavedIngredient));
        #endregion
    }

    [Fact]
    public async Task CreateIngredient_DuplicateEntity_ThrowsArgumentException()
    {
        #region Arrange
        Ingredient unSavedIngredient = new IngredientBuilder().Build();
        Ingredient sameIngredient = unSavedIngredient;
        #endregion

        #region Act
        await _ingredientRepository.Create(unSavedIngredient);
        #endregion

        #region Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _ingredientRepository.Create(sameIngredient));
        #endregion
    }

    [Fact]
    public async Task CreateIngredient_WithRelations_EnsuresCorrectForeignKeysAndSavesRelatedEntities()
    {
        #region Arrange
        const int NUMBER_OF_CREATED_RECIPES = 3;
        List<Recipe> recipes = RecipeBuilder.BuildMany(NUMBER_OF_CREATED_RECIPES);

        _dbContext.Recipes.AddRange(recipes);
        await _dbContext.SaveChangesAsync();

        Ingredient unSavedIngredient = new IngredientBuilder()
                                    .WithName("Tomatoes")
                                    .WithRecipes(recipes.ToArray())
                                    .Build();
        #endregion

        #region Act
        Ingredient savedIngredient = await _ingredientRepository.Create(unSavedIngredient);
        #endregion

        #region Assert
        Ingredient? retrievedIngredient = await _ingredientRepository.GetOneWithAllDetails(savedIngredient.ID);
        Assert.NotNull(retrievedIngredient);
        Assert.Equal(unSavedIngredient.Name, retrievedIngredient!.Name);

        Assert.Equal(recipes.Count, retrievedIngredient.Recipes!.Count);
        foreach (var recipe in recipes)
        {
            Assert.Contains(retrievedIngredient.Recipes, r => r.Title == recipe.Title && r.Description == recipe.Description);
        }
        #endregion
    }


    [Fact]
    public async Task CreateIngredient_InvalidForeignKey_ThrowsDbUpdateException()
    {
        #region Arrange
        Recipe recipe = new RecipeBuilder().Build();
        recipe.ID = 3892; // invalid ID

        Ingredient unSavedIngredient = new IngredientBuilder()
            .WithName("Invalid Ingredient")
            .WithRecipes(recipe)
            .Build();
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => _ingredientRepository.Create(unSavedIngredient));
        #endregion
    }
}