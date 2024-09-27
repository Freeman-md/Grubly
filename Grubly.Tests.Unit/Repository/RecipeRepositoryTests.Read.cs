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
        Recipe recipe = new RecipeBuilder().Build();
        Recipe newRecipe = await _recipeRepository.Create(recipe);
        #endregion

        #region Act
        Recipe? savedRecipe = await _recipeRepository.GetOne(newRecipe.ID);
        #endregion

        #region Assert
        Assert.NotNull(savedRecipe);
        Assert.Equal(newRecipe.Title, savedRecipe.Title);
        Assert.Equal(newRecipe.Description, savedRecipe.Description);
        #endregion
    }

    [Fact]
    public async Task GetRecipeByTitle_ValidTitle_ReturnsCorrectRecipe()
    {
        #region Arrange
        Recipe recipe = new RecipeBuilder().Build();
        Recipe newRecipe = await _recipeRepository.Create(recipe);
        #endregion

        #region Act
        Recipe? savedRecipe = await _recipeRepository.GetOne(newRecipe.Title);
        #endregion

        #region Assert
        Assert.NotNull(savedRecipe);
        Assert.Equal(newRecipe.Title, savedRecipe.Title);
        Assert.Equal(newRecipe.Description, savedRecipe.Description);
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
            Recipe[] recipes = {
                new RecipeBuilder().WithTitle($"Tomato Omelette").Build(),
                new RecipeBuilder().WithTitle($"Cheese and Garlic").Build(),
                new RecipeBuilder().WithTitle($"Raisin Baisin").Build()
            };

            foreach (Recipe recipe in recipes) {
                await _recipeRepository.Create(recipe);
            }
        #endregion

        #region Act
            IReadOnlyList<Recipe> savedRecipes = await _recipeRepository.GetAll();
        #endregion

        #region Assert
            Assert.NotNull(savedRecipes);
            Assert.Equal(recipes.Length, savedRecipes.Count);
            Assert.Contains(savedRecipes, (recipe) => recipe.Title.Equals(recipes[0].Title));
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
