using System;
using Grubly.Models;
using Grubly.Tests.Unit.Builders;

namespace Grubly.Tests.Unit.Repository;

public partial class IngredientRepositoryTests
{
    [Fact]
    public async Task GetIngredientById_ValidId_ReturnsCorrectIngredient()
    {
        var (ingredientRepository, dbContext) = CreateScope();

        #region Arrange
        Ingredient unSavedIngredient = new IngredientBuilder().Build();
        Ingredient savedIngredient = await ingredientRepository.Create(unSavedIngredient);
        #endregion

        #region Act
        Ingredient? retrievedIngredient = await ingredientRepository.GetOne(savedIngredient.ID);
        #endregion

        #region Assert
        Assert.NotNull(retrievedIngredient);
        Assert.Equal(savedIngredient.Name, retrievedIngredient.Name);
        Assert.Equal(savedIngredient.Description, retrievedIngredient.Description);
        #endregion
    }

    [Fact]
    public async Task GetIngredientByName_ValidName_ReturnsCorrectIngredient()
    {
        var (ingredientRepository, dbContext) = CreateScope();

        #region Arrange
        Ingredient unSavedIngredient = new IngredientBuilder().Build();
        Ingredient savedIngredient = await ingredientRepository.Create(unSavedIngredient);
        #endregion

        #region Act
        Ingredient? retrievedIngredient = await ingredientRepository.GetOne(savedIngredient.Name);
        #endregion

        #region Assert
        Assert.NotNull(retrievedIngredient);
        Assert.Equal(savedIngredient.Name, retrievedIngredient.Name);
        Assert.Equal(savedIngredient.Description, retrievedIngredient.Description);
        #endregion
    }

    [Fact]
    public async Task GetOneWithAllDetails_ByID_ReturnsIngredientWithRelatedRecipes()
    {
        var (ingredientRepository, dbContext) = CreateScope();

        #region Arrange
        // Create related recipes
        List<Recipe> recipes = RecipeBuilder.BuildMany(3);

        // Add and save recipes
        dbContext.Recipes.AddRange(recipes);
        await dbContext.SaveChangesAsync();

        // Create and save an ingredient with recipes
        Ingredient savedIngredient = await ingredientRepository.Create(
            new IngredientBuilder()
            .WithName("Tomatoes")
            .WithRecipes(recipes.ToArray())
            .Build()
        );
        #endregion

        #region Act
        Ingredient? retrievedIngredient = await ingredientRepository.GetOneWithAllDetails(savedIngredient.ID);
        #endregion

        #region Assert
        Assert.NotNull(retrievedIngredient);
        Assert.Equal(savedIngredient.Name, retrievedIngredient!.Name);
        Assert.Equal(recipes.Count, retrievedIngredient.Recipes.Count); // Ensure recipes are loaded
        foreach (var recipe in recipes)
        {
            Assert.Contains(retrievedIngredient.Recipes, r => r.Title == recipe.Title && r.Description == recipe.Description);
        }
        #endregion
    }

    [Fact]
    public async Task GetOneWithAllDetails_ByName_ReturnsIngredientWithRelatedRecipes()
    {
        var (ingredientRepository, dbContext) = CreateScope();

        #region Arrange
        // Create related recipes
        List<Recipe> recipes = RecipeBuilder.BuildMany(3);

        // Add and save recipes
        dbContext.Recipes.AddRange(recipes);
        await dbContext.SaveChangesAsync();

        // Create and save an ingredient with recipes
        Ingredient savedIngredient = await ingredientRepository.Create(
            new IngredientBuilder()
            .WithName("Garlic")
            .WithRecipes(recipes.ToArray())
            .Build()
        );
        #endregion

        #region Act
        Ingredient? retrievedIngredient = await ingredientRepository.GetOneWithAllDetails(savedIngredient.Name);
        #endregion

        #region Assert
        Assert.NotNull(retrievedIngredient);
        Assert.Equal(savedIngredient.Name, retrievedIngredient!.Name);
        Assert.Equal(recipes.Count, retrievedIngredient.Recipes.Count); // Ensure recipes are loaded
        foreach (var recipe in recipes)
        {
            Assert.Contains(retrievedIngredient.Recipes, r => r.Title == recipe.Title && r.Description == recipe.Description);
        }
        #endregion
    }



    [Fact]
    public async Task GetIngredientById_InvalidId_ReturnsNull()
    {
        var (ingredientRepository, dbContext) = CreateScope();

        #region Arrange
        Ingredient ingredient = new IngredientBuilder().Build();
        ingredient.ID = 893; // random ID
        #endregion

        #region Act
        Ingredient? nullIngredient = await ingredientRepository.GetOne(ingredient.ID);
        #endregion

        #region Assert
        Assert.Null(nullIngredient);
        #endregion
    }

    [Fact]
    public async Task GetIngredientByName_InvalidName_ReturnsNull()
    {
        var (ingredientRepository, dbContext) = CreateScope();

        #region Arrange
        Ingredient ingredient = new IngredientBuilder().Build();
        #endregion

        #region Act
        Ingredient? nullIngredient = await ingredientRepository.GetOne(ingredient.Name);
        #endregion

        #region Assert
        Assert.Null(nullIngredient);
        #endregion
    }

    [Fact]
    public async Task GetAllIngredients_ReturnsAllIngredients()
    {
        var (ingredientRepository, dbContext) = CreateScope();

        #region Arrange
        Ingredient[] unSavedIngredients = {
                new IngredientBuilder().WithName($"Tomato").Build(),
                new IngredientBuilder().WithName($"Cheese").Build(),
                new IngredientBuilder().WithName($"Raisin").Build()
            };

        foreach (Ingredient ingredient in unSavedIngredients)
        {
            await ingredientRepository.Create(ingredient);
        }
        #endregion

        #region Act
        IReadOnlyList<Ingredient> savedIngredients = await ingredientRepository.GetAll();
        #endregion

        #region Assert
        Assert.NotNull(savedIngredients);
        Assert.Equal(unSavedIngredients.Length, savedIngredients.Count);
        Assert.Contains(savedIngredients, (ingredient) => ingredient.Name.Equals(unSavedIngredients[0].Name));
        #endregion
    }

    [Fact]
    public async Task GetAllIngredients_EmptyDatabase_ReturnsEmptyList()
    {
        var (ingredientRepository, dbContext) = CreateScope();

        #region Act
        IReadOnlyList<Ingredient> noIngredients = await ingredientRepository.GetAll();
        #endregion

        #region Assert
        Assert.NotNull(noIngredients);
        Assert.Empty(noIngredients);
        #endregion
    }
}
