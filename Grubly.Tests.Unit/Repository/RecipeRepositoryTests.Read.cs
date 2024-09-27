using System;
using Grubly.Models;
using Grubly.Tests.Unit.Builders;

namespace Grubly.Tests.Unit.Repository;

public partial class RecipeRepositoryTests
{
    [Fact]
    public async Task GetRecipeById_ValidId_ReturnsCorrectRecipe()
    {
        #region Arrange
        Recipe unSavedRecipe = new RecipeBuilder().Build();
        Recipe savedRecipe = await _recipeRepository.Create(unSavedRecipe);
        #endregion

        #region Act
        Recipe? retrievedRecipe = await _recipeRepository.GetOne(savedRecipe.ID);
        #endregion

        #region Assert
        Assert.NotNull(retrievedRecipe);
        Assert.Equal(savedRecipe.Title, retrievedRecipe.Title);
        Assert.Equal(savedRecipe.Description, retrievedRecipe.Description);
        #endregion
    }

    [Fact]
    public async Task GetRecipeByTitle_ValidTitle_ReturnsCorrectRecipe()
    {
        #region Arrange
        Recipe unSavedRecipe = new RecipeBuilder().Build();
        Recipe savedRecipe = await _recipeRepository.Create(unSavedRecipe);
        #endregion

        #region Act
        Recipe? retrievedRecipe = await _recipeRepository.GetOne(savedRecipe.Title);
        #endregion

        #region Assert
        Assert.NotNull(retrievedRecipe);
        Assert.Equal(savedRecipe.Title, retrievedRecipe.Title);
        Assert.Equal(savedRecipe.Description, retrievedRecipe.Description);
        #endregion
    }

    [Fact]
    public async Task GetRecipeById_InvalidId_ReturnsNull()
    {
        #region Arrange
        Recipe recipe = new RecipeBuilder().Build();
        recipe.ID = 893; // random ID
        #endregion

        // Get recipe by id when not saved to the database - this should return null
        #region Act
        Recipe? nullRecipe = await _recipeRepository.GetOne(recipe.ID);
        #endregion

        #region Assert
        Assert.Null(nullRecipe);
        #endregion
    }

    [Fact]
    public async Task GetRecipeByTitle_InvalidTitle_ReturnsNull()
    {
        #region Arrange
        Recipe recipe = new RecipeBuilder().Build();
        #endregion

        // Get recipe by title when not saved to the database - this should return null
        #region Act
        Recipe? nullRecipe = await _recipeRepository.GetOne(recipe.Title);
        #endregion

        #region Assert
        Assert.Null(nullRecipe);
        #endregion
    }

    [Fact]
    public async Task GetAllRecipes_ReturnsAllRecipes() {
        #region Arrange
            Recipe[] unSavedRecipes = {
                new RecipeBuilder().WithTitle($"Tomato Omelette").Build(),
                new RecipeBuilder().WithTitle($"Cheese and Garlic").Build(),
                new RecipeBuilder().WithTitle($"Raisin Baisin").Build()
            };

            foreach (Recipe recipe in unSavedRecipes) {
                await _recipeRepository.Create(recipe);
            }
        #endregion

        #region Act
            IReadOnlyList<Recipe> savedRecipes = await _recipeRepository.GetAll();
        #endregion

        #region Assert
            Assert.NotNull(savedRecipes);
            Assert.Equal(unSavedRecipes.Length, savedRecipes.Count);
            Assert.Contains(savedRecipes, (recipe) => recipe.Title.Equals(unSavedRecipes[0].Title));
        #endregion
    }

     [Fact]
    public async Task GetAllRecipes_EmptyDatabase_ReturnsEmptyList() {
        #region Act
            IReadOnlyList<Recipe> noRecipes = await _recipeRepository.GetAll();
        #endregion

        #region Assert
            Assert.NotNull(noRecipes);
            Assert.Empty(noRecipes);
        #endregion
    }
}
