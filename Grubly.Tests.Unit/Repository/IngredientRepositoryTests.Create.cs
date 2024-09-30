using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Grubly.Data;
using Grubly.Interfaces.Repositories;
using Grubly.Models;
using Grubly.Repositories;
using Grubly.Tests.Unit.Builders;
using Grubly.Tests.Unit.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Grubly.Tests.Unit.Repository;

public partial class IngredientRepositoryTests : IClassFixture<TestFixture>
{
    private readonly ServiceProvider _serviceProvider;

    public IngredientRepositoryTests(TestFixture fixture)
    {
        _serviceProvider = fixture.ServiceProvider;
    }

    private (IIngredientRepository ingredientRepository, GrublyContext dbContext) CreateScope()
    {
        var scope = _serviceProvider.CreateScope();
        var scopedServices = scope.ServiceProvider;

        var ingredientRepository = scopedServices.GetRequiredService<IIngredientRepository>();
        var dbContext = scopedServices.GetRequiredService<GrublyContext>();

        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        return (ingredientRepository, dbContext);
    }

    [Fact]
    public async Task CreateIngredient_ValidInput_AddsIngredientToDatabase()
    {
        var (ingredientRepository, dbContext) = CreateScope();

        #region Arrange
        Ingredient unSavedIngredient = new IngredientBuilder().Build();
        #endregion

        #region Act
        Ingredient savedIngredient = await ingredientRepository.Create(unSavedIngredient);
        #endregion

        #region Assert
        Assert.NotNull(savedIngredient);
        Assert.True(savedIngredient.ID > 0, "The Ingredient ID should be greater than 0 after saving to the database.");
        Assert.Equal(unSavedIngredient.Name, savedIngredient.Name);
        #endregion
    }

    [Fact]
    public async Task CreateIngredient_NullInput_ThrowsArgumentNullException()
    {
        var (ingredientRepository, dbContext) = CreateScope();

        #region Arrange
        Ingredient? nullIngredient = null;
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => ingredientRepository.Create(nullIngredient!));
        #endregion
    }

    [Theory]
    [InlineData(null, "Valid Description")]
    public async Task CreateIngredient_InvalidInputs_ThrowsDbUpdateException(string title, string description)
    {
        var (ingredientRepository, dbContext) = CreateScope();

        #region Arrange
        Ingredient unSavedIngredient = new Ingredient { Name = title, Description = description };
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => ingredientRepository.Create(unSavedIngredient));
        #endregion
    }

    [Fact]
    public async Task CreateIngredient_DuplicateEntity_ThrowsDbUpdateException()
    {
        var (ingredientRepository, dbContext) = CreateScope();

        #region Arrange
        Ingredient originalIngredient = new IngredientBuilder().Build();
        Ingredient duplicateIngredient = new Ingredient
        {
            Name = originalIngredient.Name,
            Description = originalIngredient.Description
        };
        #endregion

        #region Act
        await ingredientRepository.Create(originalIngredient);
        #endregion

        #region Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => ingredientRepository.Create(duplicateIngredient));
        #endregion
    }


    [Fact]
    public async Task CreateIngredient_InvalidForeignKey_ThrowsDbUpdateException()
    {
        var (ingredientRepository, dbContext) = CreateScope();

        #region Arrange
        Recipe recipe = new RecipeBuilder().Build();
        recipe.ID = 3892; // invalid ID

        Ingredient unSavedIngredient = new IngredientBuilder()
            .WithName("Invalid Ingredient")
            .WithRecipes(recipe)
            .Build();
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => ingredientRepository.Create(unSavedIngredient));
        #endregion
    }

    //     [Fact]
    // public async Task CreateIngredient_WithRelations_EnsuresCorrectForeignKeysAndSavesRelatedEntities()
    // {
    //     var (ingredientRepository, dbContext) = CreateScope();

    //     #region Arrange
    //     const int NUMBER_OF_CREATED_RECIPES = 3;
    //     List<Recipe> recipes = RecipeBuilder.BuildMany(NUMBER_OF_CREATED_RECIPES);

    //     dbContext.Recipes.AddRange(recipes);
    //     await dbContext.SaveChangesAsync();

    //     Ingredient unSavedIngredient = new IngredientBuilder()
    //                                 .WithName("Tomatoes")
    //                                 .WithRecipes(recipes.ToArray())
    //                                 .Build();
    //     #endregion

    //     #region Act
    //     Ingredient savedIngredient = await ingredientRepository.Create(unSavedIngredient);
    //     #endregion

    //     #region Assert
    //     Assert.True(savedIngredient.ID > 0, "The Ingredient ID should be greater than 0 after saving to the database.");

    //     // get model directly from db using repository to ensure relations were saved
    //     Ingredient? retrievedIngredient = await ingredientRepository.GetOneWithAllDetails(savedIngredient.ID);
    //     Assert.NotNull(retrievedIngredient);
    //     Assert.Equal(unSavedIngredient.Name, retrievedIngredient!.Name);

    //     Assert.Equal(recipes.Count, retrievedIngredient.Recipes!.Count);
    //     foreach (var recipe in recipes)
    //     {
    //         Assert.Contains(retrievedIngredient.Recipes, r => r.Title == recipe.Title && r.Description == recipe.Description);
    //     }
    //     #endregion
    // }
}