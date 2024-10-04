using System;
using Grubly.Models;
using Grubly.Services;
using Grubly.Tests.Unit.Builders;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace Grubly.Tests.Services;

public partial class IngredientServiceTests
{
    [Fact]
    public async Task UpdateIngredient_ValidInput_UpdatesIngredientSuccessfully()
    {
        var (ingredientService, dbContext) = CreateScope();

        #region Arrange
        Ingredient unSavedIngredient = new IngredientBuilder().Build();

        Ingredient savedIngredient = await ingredientService.CreateIngredient(unSavedIngredient);

        dbContext.Entry(savedIngredient).State = EntityState.Detached;
        #endregion

        #region Act
        savedIngredient.Name = "Tomato and Garlic";
        savedIngredient.Description = "For making stew";

        Ingredient updatedIngredient = await ingredientService.UpdateIngredient(savedIngredient, savedIngredient.ID);
        #endregion

        #region Assert
        Ingredient? retrievedIngredient = await ingredientService.GetIngredient(updatedIngredient.ID);

        Assert.NotEqual(unSavedIngredient.Name, retrievedIngredient!.Name);

        Assert.Equal(updatedIngredient.ID, retrievedIngredient.ID);
        Assert.Equal(updatedIngredient.Name, retrievedIngredient.Name);
        Assert.Equal(updatedIngredient.Description, retrievedIngredient.Description);
        #endregion
    }

    [Fact]
    public async Task UpdateIngredient_NullInput_ThrowsArgumentNullException()
    {
        var (ingredientService, dbContext) = CreateScope();

        #region Act -> Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => ingredientService.UpdateIngredient(null!, 1));
        #endregion
    }

    [Fact]
    public async Task UpdateIngredient_InvalidId_ThrowsKeyNotFoundException()
    {
        var (ingredientService, dbContext) = CreateScope();

        Ingredient randomIngredient = new IngredientBuilder().Build();
        int randomIngredientId = (new Random()).Next(3892, 389289);

        #region Act -> Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => ingredientService.UpdateIngredient(randomIngredient, randomIngredientId));
        #endregion
    }

    [Fact]
    public async Task UpdateIngredient_DuplicateName_ThrowsDbUpdateException()
    {
        var (ingredientService, dbContext) = CreateScope();

        #region Arrange
        Ingredient firstIngredient = await ingredientService.CreateIngredient(new IngredientBuilder().WithName("Tomato and Garlic").Build());
        Ingredient secondIngredient = await ingredientService.CreateIngredient(new IngredientBuilder().WithName("British Peppers").Build());

        dbContext.Entry(secondIngredient).State = EntityState.Detached;
        #endregion

        #region Act
        secondIngredient.Name = firstIngredient.Name;
        #endregion

        #region Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => ingredientService.UpdateIngredient(secondIngredient, secondIngredient.ID));
        #endregion
    }

    [Fact]
    public async Task UpdateIngredient_ExistingRelations_PresevesRelationships()
    {
        var (ingredientService, dbContext) = CreateScope();

        #region Arrange
        const string UPDATED_INGREDIENT_NAME = "Tomato and Garlic";
        const string UPDATED_INGREDIENT_DESCRIPTION = "For making the best stew";

        List<Recipe> recipes = RecipeBuilder.BuildMany(3);

        Ingredient unSavedIngredient = new IngredientBuilder()
                                     .WithRecipes(recipes.ToArray())
                                     .Build();

        await dbContext.Recipes.AddRangeAsync(recipes.ToArray());
        await dbContext.SaveChangesAsync();

        Ingredient savedIngredient = await ingredientService.CreateIngredient(unSavedIngredient);

        dbContext.Entry(savedIngredient).State = EntityState.Detached;
        #endregion

        #region Act
        savedIngredient.Name = UPDATED_INGREDIENT_NAME;
        savedIngredient.Description = UPDATED_INGREDIENT_DESCRIPTION;

        Ingredient updatedIngredient = await ingredientService.UpdateIngredient(savedIngredient, savedIngredient.ID);
        #endregion

        #region Assert
        Ingredient? retrievedIngredient = await ingredientService.GetIngredient(savedIngredient.ID);

        Assert.NotEqual(unSavedIngredient.Name, retrievedIngredient!.Name);

        Assert.Equal(updatedIngredient.ID, retrievedIngredient.ID);
        Assert.Equal(updatedIngredient.Name, retrievedIngredient.Name);
        Assert.Equal(updatedIngredient.Description, retrievedIngredient.Description);

        foreach (var recipe in updatedIngredient.Recipes)
        {
            Assert.Contains(retrievedIngredient.Recipes, (r) => r.Title == recipe.Title && r.Description == recipe.Description);
        }
        #endregion
    }

    [Fact]
    public async Task UpdateIngredient_WithNewRecipes_AddsNewRelations()
    {
        var (ingredientService, dbContext) = CreateScope();

        #region Arrange
        const string UPDATED_INGREDIENT_NAME = "Tomato and Garlic";
        const string UPDATED_INGREDIENT_DESCRIPTION = "For making the best stew";

        List<Recipe> initialRecipes = RecipeBuilder.BuildMany(3);
        List<Recipe> additionalRecipes = RecipeBuilder.BuildMany(3);

        Ingredient unSavedIngredient = new IngredientBuilder()
                                     .WithRecipes(initialRecipes.ToArray())
                                     .Build();

        await dbContext.Recipes.AddRangeAsync(initialRecipes.ToArray().Concat(additionalRecipes.ToArray()));
        await dbContext.SaveChangesAsync();

        Ingredient savedIngredient = await ingredientService.CreateIngredient(unSavedIngredient);

        dbContext.Entry(savedIngredient).State = EntityState.Detached;
        #endregion

        #region Act
        savedIngredient.Name = UPDATED_INGREDIENT_NAME;
        savedIngredient.Description = UPDATED_INGREDIENT_DESCRIPTION;
        savedIngredient.Recipes.AddRange(additionalRecipes);

        Ingredient updatedIngredient = await ingredientService.UpdateIngredient(savedIngredient, savedIngredient.ID);
        #endregion

        #region Assert
        Ingredient? retrievedIngredient = await ingredientService.GetIngredient(savedIngredient.ID);
        List<Recipe> expectedRecipes = initialRecipes.Concat(additionalRecipes).ToList();

        Assert.NotEqual(unSavedIngredient.Name, retrievedIngredient!.Name);

        Assert.Equal(updatedIngredient.ID, retrievedIngredient.ID);
        Assert.Equal(updatedIngredient.Name, retrievedIngredient.Name);
        Assert.Equal(updatedIngredient.Description, retrievedIngredient.Description);

        Assert.Equal(expectedRecipes.Count, retrievedIngredient.Recipes!.Count);
        foreach (var recipe in updatedIngredient.Recipes)
        {
            Assert.Contains(retrievedIngredient.Recipes, r => r.Title == recipe.Title);
        }
        #endregion
    }

    [Fact]
    public async Task UpdateIngredient_RemovesOldRecipesFromIngredient() {
        var (ingredientService, dbContext) = CreateScope();

        #region Arrange
        const string UPDATED_INGREDIENT_NAME = "Tomato and Garlic";
        const string UPDATED_INGREDIENT_DESCRIPTION = "For making the best stew";

        List<Recipe> initialRecipes = RecipeBuilder.BuildMany(3);
        List<Recipe> newRecipes = RecipeBuilder.BuildMany(3);

        Ingredient unSavedIngredient = new IngredientBuilder()
                                     .WithRecipes(initialRecipes.ToArray())
                                     .Build();

        await dbContext.Recipes.AddRangeAsync(initialRecipes.ToArray().Concat(newRecipes.ToArray()));
        await dbContext.SaveChangesAsync();

        Ingredient savedIngredient = await ingredientService.CreateIngredient(unSavedIngredient);

        dbContext.Entry(savedIngredient).State = EntityState.Detached;
        #endregion

        #region Act
        savedIngredient.Name = UPDATED_INGREDIENT_NAME;
        savedIngredient.Description = UPDATED_INGREDIENT_DESCRIPTION;
        savedIngredient.Recipes = newRecipes;

        Ingredient updatedIngredient = await ingredientService.UpdateIngredient(savedIngredient, savedIngredient.ID);
        #endregion

        #region Assert
        Ingredient? retrievedIngredient = await ingredientService.GetIngredient(savedIngredient.ID);

        Assert.NotEqual(unSavedIngredient.Name, retrievedIngredient!.Name);

        Assert.Equal(updatedIngredient.ID, retrievedIngredient.ID);
        Assert.Equal(updatedIngredient.Name, retrievedIngredient.Name);
        Assert.Equal(updatedIngredient.Description, retrievedIngredient.Description);

        Assert.Equal(newRecipes.Count, retrievedIngredient.Recipes!.Count);
        foreach (var recipe in newRecipes)
        {
            Assert.Contains(retrievedIngredient.Recipes, r => r.Title == recipe.Title);
        }

        foreach (var recipe in initialRecipes)
        {
            Assert.DoesNotContain(retrievedIngredient.Recipes, r => r.Title == recipe.Title);
        }
        #endregion
    }

}
