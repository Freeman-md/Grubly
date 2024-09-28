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
        #region Arrange
        Ingredient savedIngredient = await _ingredientRepository.Create(new IngredientBuilder().Build());
        #endregion

        #region Act
        await _ingredientRepository.Delete(savedIngredient.ID);
        #endregion

        #region Assert
        Ingredient? nullIngredient = await _ingredientRepository.GetOne(savedIngredient.ID);
        Assert.Null(nullIngredient);
        #endregion
    }

    [Fact]
    public async Task DeleteIngredient_InvalidId_ThrowsNotFoundException()
    {
        //TODO: Create a NotFoundException Class in main project and use here

        #region Arrange
        Ingredient savedIngredient = await _ingredientRepository.Create(new IngredientBuilder().Build());
        await _ingredientRepository.Delete(savedIngredient.ID);
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await _ingredientRepository.Delete(savedIngredient.ID));
        #endregion
    }

    [Fact]
    public async Task DeleteIngredient_WithManyToManyRelationships_RemovesLinksButKeepsEntities()
    {
        #region Arrange
        const int NUMBER_OF_CREATED_RECIPES = 3;
        List<Recipe> recipes = RecipeBuilder.BuildMany(NUMBER_OF_CREATED_RECIPES);

        _dbContext.Recipes.AddRange(recipes);
        await _dbContext.SaveChangesAsync();

        Ingredient savedIngredient = await _ingredientRepository.Create(
                                                            new IngredientBuilder()
                                                            .WithRecipes(recipes.ToArray())
                                                            .Build()
                                                            );
        #endregion

        #region Act
        await _ingredientRepository.Delete(savedIngredient.ID);
        #endregion

        #region Assert
        Ingredient? nullIngredient = await _ingredientRepository.GetOne(savedIngredient.ID);
        Assert.Null(nullIngredient);

        foreach (var recipe in recipes)
        {
            Recipe? existingRecipe = await _dbContext.Recipes.FirstOrDefaultAsync((recipe) => recipe.Title == recipe.Title);
            Assert.NotNull(existingRecipe);

            Assert.DoesNotContain(existingRecipe.Ingredients, (recipe) => recipe.ID == savedIngredient.ID);
        }
        #endregion
    }
}
