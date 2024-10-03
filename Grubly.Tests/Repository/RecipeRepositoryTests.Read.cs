using System;
using Grubly.Models;
using Grubly.Tests.Unit.Builders;

namespace Grubly.Tests.Unit.Repository;

public partial class RecipeRepositoryTests
{
    [Fact]
    public async Task GetRecipeById_ValidId_ReturnsCorrectRecipe()
    {
        var (recipeRepository, dbContext) = CreateScope();

        #region Arrange
        Recipe unSavedRecipe = new RecipeBuilder().Build();
        Recipe savedRecipe = await recipeRepository.Create(unSavedRecipe);
        #endregion

        #region Act
        Recipe? retrievedRecipe = await recipeRepository.GetOne(savedRecipe.ID);
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
        var (recipeRepository, dbContext) = CreateScope();

        #region Arrange
        Recipe unSavedRecipe = new RecipeBuilder().Build();
        Recipe savedRecipe = await recipeRepository.Create(unSavedRecipe);
        #endregion

        #region Act
        Recipe? retrievedRecipe = await recipeRepository.GetOne(savedRecipe.Title);
        #endregion

        #region Assert
        Assert.NotNull(retrievedRecipe);
        Assert.Equal(savedRecipe.Title, retrievedRecipe.Title);
        Assert.Equal(savedRecipe.Description, retrievedRecipe.Description);
        #endregion
    }

    [Fact]
    public async Task GetOneWithAllDetails_ByID_ReturnsRecipeWithRelatedIngredientsAndCategories()
    {
        var (recipeRepository, dbContext) = CreateScope();

        #region Arrange
        // Create related ingredients and categories
        List<Ingredient> ingredients = IngredientBuilder.BuildMany(2);

        List<Category> categories = CategoryBuilder.BuildMany(2);

        // Add and save ingredients and categories
        dbContext.Ingredients.AddRange(ingredients);
        dbContext.Categories.AddRange(categories);
        await dbContext.SaveChangesAsync();

        // Create and save a recipe with ingredients and categories
        Recipe savedRecipe = await recipeRepository.Create(
            new RecipeBuilder()
            .WithTitle("Delicious Pasta")
            .WithIngredients(ingredients.ToArray())
            .WithCategories(categories.ToArray())
            .Build()
        );
        #endregion

        #region Act
        Recipe? retrievedRecipe = await recipeRepository.GetOneWithAllDetails(savedRecipe.ID);
        #endregion

        #region Assert
        Assert.NotNull(retrievedRecipe);
        Assert.Equal(savedRecipe.Title, retrievedRecipe!.Title);

        // Ensure ingredients are loaded
        Assert.Equal(ingredients.Count, retrievedRecipe.Ingredients.Count);
        foreach (var ingredient in ingredients)
        {
            Assert.Contains(retrievedRecipe.Ingredients, i => i.Name == ingredient.Name && i.Description == ingredient.Description);
        }

        // Ensure categories are loaded
        Assert.Equal(categories.Count, retrievedRecipe.Categories.Count);
        foreach (var category in categories)
        {
            Assert.Contains(retrievedRecipe.Categories, c => c.Name == category.Name);
        }
        #endregion
    }

    [Fact]
    public async Task GetOneWithAllDetails_ByTitle_ReturnsRecipeWithRelatedIngredientsAndCategories()
    {
        var (recipeRepository, dbContext) = CreateScope();

        #region Arrange
        // Create related ingredients and categories
        List<Ingredient> ingredients = IngredientBuilder.BuildMany(2);

        List<Category> categories = CategoryBuilder.BuildMany(2);

        // Add and save ingredients and categories
        dbContext.Ingredients.AddRange(ingredients);
        dbContext.Categories.AddRange(categories);
        await dbContext.SaveChangesAsync();

        // Create and save a recipe with ingredients and categories
        Recipe savedRecipe = await recipeRepository.Create(
            new RecipeBuilder()
            .WithTitle("Hearty Soup")
            .WithIngredients(ingredients.ToArray())
            .WithCategories(categories.ToArray())
            .Build()
        );
        #endregion

        #region Act
        Recipe? retrievedRecipe = await recipeRepository.GetOneWithAllDetails(savedRecipe.Title);
        #endregion

        #region Assert
        Assert.NotNull(retrievedRecipe);
        Assert.Equal(savedRecipe.Title, retrievedRecipe!.Title);

        // Ensure ingredients are loaded
        Assert.Equal(ingredients.Count, retrievedRecipe.Ingredients.Count);
        foreach (var ingredient in ingredients)
        {
            Assert.Contains(retrievedRecipe.Ingredients, i => i.Name == ingredient.Name && i.Description == ingredient.Description);
        }

        // Ensure categories are loaded
        Assert.Equal(categories.Count, retrievedRecipe.Categories.Count);
        foreach (var category in categories)
        {
            Assert.Contains(retrievedRecipe.Categories, c => c.Name == category.Name);
        }
        #endregion
    }



    [Fact]
    public async Task GetRecipeById_InvalidId_ReturnsNull()
    {
        var (recipeRepository, dbContext) = CreateScope();

        #region Arrange
        Recipe recipe = new RecipeBuilder().Build();
        recipe.ID = 893; // random ID
        #endregion

        // Get recipe by id when not saved to the database - this should return null
        #region Act
        Recipe? nullRecipe = await recipeRepository.GetOne(recipe.ID);
        #endregion

        #region Assert
        Assert.Null(nullRecipe);
        #endregion
    }

    [Fact]
    public async Task GetRecipeByTitle_InvalidTitle_ReturnsNull()
    {
        var (recipeRepository, dbContext) = CreateScope();

        #region Arrange
        Recipe recipe = new RecipeBuilder().Build();
        #endregion

        // Get recipe by title when not saved to the database - this should return null
        #region Act
        Recipe? nullRecipe = await recipeRepository.GetOne(recipe.Title);
        #endregion

        #region Assert
        Assert.Null(nullRecipe);
        #endregion
    }

    [Fact]
    public async Task GetAllRecipes_ReturnsAllRecipes()
    {
        var (recipeRepository, dbContext) = CreateScope();

        #region Arrange
        Recipe[] unSavedRecipes = {
                new RecipeBuilder().WithTitle($"Tomato Omelette").Build(),
                new RecipeBuilder().WithTitle($"Cheese and Garlic").Build(),
                new RecipeBuilder().WithTitle($"Raisin Baisin").Build()
            };

        foreach (Recipe recipe in unSavedRecipes)
        {
            await recipeRepository.Create(recipe);
        }
        #endregion

        #region Act
        IReadOnlyList<Recipe> savedRecipes = await recipeRepository.GetAll();
        #endregion

        #region Assert
        Assert.NotNull(savedRecipes);
        Assert.Equal(unSavedRecipes.Length, savedRecipes.Count);
        Assert.Contains(savedRecipes, (recipe) => recipe.Title.Equals(unSavedRecipes[0].Title));
        #endregion
    }

    [Fact]
    public async Task GetAllRecipes_EmptyDatabase_ReturnsEmptyList()
    {
        var (recipeRepository, dbContext) = CreateScope();

        #region Act
        IReadOnlyList<Recipe> noRecipes = await recipeRepository.GetAll();
        #endregion

        #region Assert
        Assert.NotNull(noRecipes);
        Assert.Empty(noRecipes);
        #endregion
    }
}
