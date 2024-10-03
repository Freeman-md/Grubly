using System;
using System.Diagnostics.CodeAnalysis;
using Grubly.Models;
using Grubly.Tests.Unit.Builders;
using Microsoft.EntityFrameworkCore;

namespace Grubly.Tests.Unit.Repository;

public partial class RecipeRepositoryTests
{
    [Fact]
    public async Task DeleteRecipe_ValidId_RemovesRecipeFromDatabase()
    {
        var (recipeRepository, dbContext) = CreateScope();
        
        #region Arrange
        Recipe savedRecipe = await recipeRepository.Create(new RecipeBuilder().Build());
        #endregion

        #region Act
        await recipeRepository.Delete(savedRecipe.ID);
        #endregion

        #region Assert
        Recipe? nullRecipe = await recipeRepository.GetOne(savedRecipe.ID);
        Assert.Null(nullRecipe);
        #endregion
    }

    [Fact]
    public async Task DeleteRecipe_InvalidId_ThrowsKeyNotFoundException()
    {
        var (recipeRepository, dbContext) = CreateScope();

        #region Arrange
        Recipe savedRecipe = await recipeRepository.Create(new RecipeBuilder().Build());
        await recipeRepository.Delete(savedRecipe.ID);
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await recipeRepository.Delete(savedRecipe.ID));
        #endregion
    }

    [Fact]
    public async Task DeleteRecipe_WithManyToManyRelationships_RemovesLinksButKeepsEntities()
    {
        var (recipeRepository, dbContext) = CreateScope();
        
        #region Arrange
        // Define ingredients
        Ingredient[] ingredients = {
            new Ingredient { Name = "Tomato", Description = "Fresh red tomatoes" },
            new Ingredient { Name = "Garlic", Description = "Fresh garlic cloves" }
        };

        // Define categories
        Category[] categories = {
            new Category { Name = "Breakfast" },
            new Category { Name = "Lunch" }
        };

        // Add ingredients and categories to the context
        dbContext.Ingredients.AddRange(ingredients);
        dbContext.Categories.AddRange(categories);
        await dbContext.SaveChangesAsync();

        Recipe savedRecipe = await recipeRepository.Create(
                                                            new RecipeBuilder()
                                                            .WithIngredients(ingredients)
                                                            .WithCategories(categories)
                                                            .Build()
                                                            );
        #endregion

        #region Act
        await recipeRepository.Delete(savedRecipe.ID);
        #endregion

        #region Assert
        Recipe? nullRecipe = await recipeRepository.GetOne(savedRecipe.ID);
        Assert.Null(nullRecipe);

        foreach (var ingredient in ingredients)
        {
            Ingredient? existingIngredient = await dbContext.Ingredients.FirstOrDefaultAsync((i) => i.Name == ingredient.Name);
            Assert.NotNull(existingIngredient);

            Assert.DoesNotContain(existingIngredient.Recipes, (recipe) => recipe.ID == savedRecipe.ID);
        }

        foreach (var category in categories)
        {
            Category? existingCategory = await dbContext.Categories.FirstOrDefaultAsync((c) => c.Name == category.Name);
            Assert.NotNull(existingCategory);

            Assert.DoesNotContain(existingCategory.Recipes, (recipe) => recipe.ID == savedRecipe.ID);
        }
        #endregion
    }
}
