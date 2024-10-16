using System;
using System.ComponentModel.DataAnnotations;
using Grubly.Models;
using Grubly.Tests.Unit.Builders;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace Grubly.Tests.Unit.Repository;

public partial class IngredientRepositoryTests
{
    [Theory]
    [InlineData("Updated Name", "Updated Description")]
    public async Task UpdateIngredient_ValidEntity_UpdatesIngredientInDatabase(string name, string description)
    {
        var (ingredientRepository, dbContext) = CreateScope();
        
        #region Arrange
        Ingredient savedIngredient = await ingredientRepository.Create(new IngredientBuilder().Build());

        dbContext.Entry(savedIngredient).State = EntityState.Detached;
        #endregion

        #region Act
        savedIngredient.Name = name;
        savedIngredient.Description = description;
        
        Ingredient updatedIngredient = await ingredientRepository.Update(savedIngredient, savedIngredient.ID);
        #endregion

        #region Assert
        Ingredient? retrievedIngredient = await ingredientRepository.GetOne(updatedIngredient.ID);

        Assert.NotNull(retrievedIngredient);

        Assert.True(updatedIngredient.ID > 0, "The Ingredient ID should be greater than 0 after saving to the database.");
        Assert.Equal(updatedIngredient.Name, retrievedIngredient!.Name);
        Assert.Equal(updatedIngredient.Description, retrievedIngredient.Description);
        #endregion
    }

    [Theory]
    [InlineData(null, "Valid Description")]
    public async Task UpdateIngredient_InvalidInputs_ThrowsDbUpdateException(string name, string description)
    {
        var (ingredientRepository, dbContext) = CreateScope();
        
        #region Arrange
        Ingredient savedIngredient = await ingredientRepository.Create(new IngredientBuilder().Build());

        dbContext.Entry(savedIngredient).State = EntityState.Detached;

        savedIngredient.Name = name;
        savedIngredient.Description = description;
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => ingredientRepository.Update(savedIngredient, savedIngredient.ID));
        #endregion
    }

    [Fact]
    public async Task UpdateIngredient_DatabaseIntegrity_MaintainsRelationships()
    {
        var (ingredientRepository, dbContext) = CreateScope();
        
        #region Arrange
        const int NUMBER_OF_CREATED_RECIPES = 3;
        const string UPDATED_NAME = "Updated Ingredient Name";
        List<Recipe> recipes = RecipeBuilder.BuildMany(NUMBER_OF_CREATED_RECIPES);

        dbContext.Recipes.AddRange(recipes);
        await dbContext.SaveChangesAsync();

        Ingredient originalIngredient = new IngredientBuilder()
            .WithName("Original Ingredient")
            .WithRecipes(recipes.ToArray())
            .Build();

        Ingredient savedIngredient = await ingredientRepository.Create(originalIngredient);

        dbContext.Entry(savedIngredient).State = EntityState.Detached;

        savedIngredient.Name = UPDATED_NAME;
        #endregion

        #region Act
        await ingredientRepository.Update(savedIngredient, savedIngredient.ID);

        Ingredient? updatedIngredient = await ingredientRepository.GetOneWithAllDetails(savedIngredient.ID);
        #endregion

        #region Assert
        Assert.NotNull(updatedIngredient);
        Assert.True(updatedIngredient.ID > 0, "The Ingredient ID should be greater than 0 after saving to the database.");
        Assert.Equal(UPDATED_NAME, updatedIngredient!.Name);

        Assert.Equal(recipes.Count, updatedIngredient.Recipes!.Count);
        #endregion
    }

    [Fact]
public async Task UpdateIngredient_ReplacesOldRecipesWithNewSet()
{
    var (ingredientRepository, dbContext) = CreateScope();

    #region Arrange
    // Initial set of recipes
    List<Recipe> initialRecipes = RecipeBuilder.BuildMany(3);

    // New set of recipes (to replace the initial set)
    List<Recipe> newRecipes = RecipeBuilder.BuildMany(2);

    dbContext.Recipes.AddRange(initialRecipes.Concat(newRecipes));
    await dbContext.SaveChangesAsync();

    // Create and save the original ingredient with the initial set of recipes
    Ingredient originalIngredient = new IngredientBuilder()
        .WithName("Original Ingredient")
        .WithRecipes(initialRecipes.ToArray())
        .Build();

    Ingredient savedIngredient = await ingredientRepository.Create(originalIngredient);

    // Detach the savedIngredient from the DbContext to stop it from being tracked
    dbContext.Entry(savedIngredient).State = EntityState.Detached;

    // Update the ingredient name and replace the old set of recipes with the new set
    savedIngredient.Name = "Updated Ingredient Name";
    savedIngredient.Recipes = newRecipes;
    #endregion

    #region Act
    await ingredientRepository.Update(savedIngredient, savedIngredient.ID);

    // Retrieve the updated ingredient with details (including recipes)
    Ingredient? updatedIngredient = await ingredientRepository.GetOneWithAllDetails(savedIngredient.ID);
    #endregion

    #region Assert
    Assert.NotNull(updatedIngredient);
    Assert.Equal("Updated Ingredient Name", updatedIngredient!.Name);

    // Verify that the old recipes have been replaced with the new ones
    Assert.Equal(newRecipes.Count, updatedIngredient.Recipes!.Count);
    foreach (var recipe in newRecipes)
    {
        Assert.Contains(updatedIngredient.Recipes, r => r.Title == recipe.Title);
    }

    // Ensure that the old recipes are no longer linked to the ingredient
    foreach (var recipe in initialRecipes)
    {
        Assert.DoesNotContain(updatedIngredient.Recipes, r => r.Title == recipe.Title);
    }
    #endregion
}

[Fact]
public async Task UpdateIngredient_AddsNewRecipesToExistingOnes()
{
    var (ingredientRepository, dbContext) = CreateScope();

    #region Arrange
    // Initial set of recipes
    List<Recipe> initialRecipes = RecipeBuilder.BuildMany(3);

    // Additional set of recipes (to be added to the initial set)
    List<Recipe> additionalRecipes = RecipeBuilder.BuildMany(2);

    dbContext.Recipes.AddRange(initialRecipes.Concat(additionalRecipes));
    await dbContext.SaveChangesAsync();

    // Create and save the original ingredient with the initial set of recipes
    Ingredient originalIngredient = new IngredientBuilder()
        .WithName("Original Ingredient")
        .WithRecipes(initialRecipes.ToArray())
        .Build();

    Ingredient savedIngredient = await ingredientRepository.Create(originalIngredient);

    // Detach the savedIngredient from the DbContext to stop it from being tracked
    dbContext.Entry(savedIngredient).State = EntityState.Detached;

    // Update the ingredient name and add new recipes while keeping the old ones
    savedIngredient.Name = "Updated Ingredient Name";
    savedIngredient.Recipes.AddRange(additionalRecipes);  // Add new recipes to the existing ones
    #endregion

    #region Act
    await ingredientRepository.Update(savedIngredient, savedIngredient.ID);

    // Retrieve the updated ingredient with details (including recipes)
    Ingredient? updatedIngredient = await ingredientRepository.GetOneWithAllDetails(savedIngredient.ID);
    #endregion

    #region Assert
    Assert.NotNull(updatedIngredient);
    Assert.Equal("Updated Ingredient Name", updatedIngredient!.Name);

    // Verify that both the initial and additional recipes are associated with the ingredient
    List<Recipe> expectedRecipes = initialRecipes.Concat(additionalRecipes).ToList();
    Assert.Equal(expectedRecipes.Count, updatedIngredient.Recipes!.Count);
    foreach (var recipe in expectedRecipes)
    {
        Assert.Contains(updatedIngredient.Recipes, r => r.Title == recipe.Title);
    }
    #endregion
}


}
