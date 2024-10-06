using System;
using Grubly.Models;
using Grubly.Tests.Unit.Builders;
using Microsoft.EntityFrameworkCore;

namespace Grubly.Tests.Unit.Repository;

public partial class IngredientRepositoryTests
{

    [Fact]
    public async Task DeleteIngredient_ValidId_RemovesIngredientFromDatabase()
    {
        var (ingredientRepository, dbContext) = CreateScope();

        #region Arrange
        Ingredient savedIngredient = await ingredientRepository.Create(new IngredientBuilder().Build());
        #endregion

        #region Act
        await ingredientRepository.Delete(savedIngredient.ID);
        #endregion

        #region Assert
        Ingredient? nullIngredient = await ingredientRepository.GetOne(savedIngredient.ID);
        Assert.Null(nullIngredient);
        #endregion
    }

    [Fact]
    public async Task DeleteIngredient_WithManyToManyRelationships_RemovesLinksButKeepsEntities()
    {
        var (ingredientRepository, dbContext) = CreateScope();

        #region Arrange
        const int NUMBER_OF_CREATED_RECIPES = 3;
        List<Recipe> recipes = RecipeBuilder.BuildMany(NUMBER_OF_CREATED_RECIPES);

        dbContext.Recipes.AddRange(recipes);
        await dbContext.SaveChangesAsync();

        Ingredient savedIngredient = await ingredientRepository.Create(
                                                            new IngredientBuilder()
                                                            .WithRecipes(recipes.ToArray())
                                                            .Build()
                                                            );
        #endregion

        #region Act
        await ingredientRepository.Delete(savedIngredient.ID);
        #endregion

        #region Assert
        Ingredient? nullIngredient = await ingredientRepository.GetOne(savedIngredient.ID);
        Assert.Null(nullIngredient);

        foreach (var recipe in recipes)
        {
            Recipe? existingRecipe = await dbContext.Recipes.FirstOrDefaultAsync((recipe) => recipe.Title == recipe.Title);
            Assert.NotNull(existingRecipe);

            Assert.DoesNotContain(existingRecipe.Ingredients, (recipe) => recipe.ID == savedIngredient.ID);
        }
        #endregion
    }
}
