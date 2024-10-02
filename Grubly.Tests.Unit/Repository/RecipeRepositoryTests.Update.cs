using System;
using System.ComponentModel.DataAnnotations;
using Grubly.Models;
using Grubly.Tests.Unit.Builders;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace Grubly.Tests.Unit.Repository;

public partial class RecipeRepositoryTests
{
    [Theory]
    [InlineData("Updated Title", "Updated Description", "Updated Instructions", "https://example.com/updated_image.jpg", CuisineType.Mexican, DifficultyLevel.Hard)]
    [InlineData("Updated Title 2", "Another Updated Description", null, null, CuisineType.Chinese, DifficultyLevel.Medium)]
    public async Task UpdateRecipe_ValidEntity_UpdatesRecipeInDatabase(string title, string description, string? instructions, string? imageUrl, CuisineType cuisineType, DifficultyLevel difficultyLevel)
    {
        var (recipeRepository, dbContext) = CreateScope();

        #region Arrange
        Recipe savedRecipe = await recipeRepository.Create(new RecipeBuilder().Build());

        dbContext.Entry(savedRecipe).State = EntityState.Detached;

        savedRecipe.Title = title;
        savedRecipe.Description = description;
        savedRecipe.Instructions = instructions;
        savedRecipe.ImageUrl = imageUrl;
        savedRecipe.CuisineType = cuisineType;
        savedRecipe.DifficultyLevel = difficultyLevel;
        #endregion

        #region Act
        Recipe updatedRecipe = await recipeRepository.Update(savedRecipe, savedRecipe.ID);
        #endregion

        #region Assert
        Recipe? retrievedRecipe = await recipeRepository.GetOne(updatedRecipe.ID);

        Assert.NotNull(retrievedRecipe);

        Assert.Equal(title, retrievedRecipe!.Title);
        Assert.Equal(description, retrievedRecipe.Description);
        Assert.Equal(instructions, retrievedRecipe.Instructions);
        Assert.Equal(imageUrl, retrievedRecipe.ImageUrl);
        Assert.Equal(cuisineType, retrievedRecipe.CuisineType);
        Assert.Equal(difficultyLevel, retrievedRecipe.DifficultyLevel);
        #endregion
    }

    [Theory]
    [InlineData(null, "Valid Description", CuisineType.Italian, DifficultyLevel.Easy)]
    [InlineData("Valid Title", null, CuisineType.Italian, DifficultyLevel.Easy)]
    public async Task UpdateRecipe_InvalidInputs_ThrowsDbUpdateException(string title, string description, CuisineType type, DifficultyLevel difficultyLevel)
    {
        var (recipeRepository, dbContext) = CreateScope();

        #region Arrange
        Recipe savedRecipe = await recipeRepository.Create(new RecipeBuilder().Build());

        dbContext.Entry(savedRecipe).State = EntityState.Detached;

        savedRecipe.Title = title;
        savedRecipe.Description = description;
        savedRecipe.CuisineType = type;
        savedRecipe.DifficultyLevel = difficultyLevel;
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => recipeRepository.Update(savedRecipe, savedRecipe.ID));
        #endregion
    }

    [Fact]
    public async Task UpdateRecipe_InvalidId_ThrowsKeyNotFoundException()
    {
        var (recipeRepository, dbContext) = CreateScope();

        //TODO: Create a NotFoundException Class in main project and use here
        #region Arrange
        const int RANDOM_ID = 82923;
        Recipe savedRecipe = await recipeRepository.Create(new RecipeBuilder().Build());

        dbContext.Entry(savedRecipe).State = EntityState.Detached;
        #endregion

        #region Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await recipeRepository.Update(savedRecipe, RANDOM_ID));
        #endregion
    }

    [Fact]
    public async Task UpdateRecipe_DatabaseIntegrity_MaintainsRelationships()
    {
        var (recipeRepository, dbContext) = CreateScope();

        #region Arrange
        // Create and save ingredients and categories
        Ingredient ingredient1 = new Ingredient { Name = "Tomato", Description = "Fresh red tomatoes" };
        Ingredient ingredient2 = new Ingredient { Name = "Garlic", Description = "Fresh garlic cloves" };
        Category category1 = new Category { Name = "Breakfast" };
        Category category2 = new Category { Name = "Lunch" };

        dbContext.Ingredients.AddRange(ingredient1, ingredient2);
        dbContext.Categories.AddRange(category1, category2);
        await dbContext.SaveChangesAsync();

        // Create and save the original recipe with relationships
        Recipe originalRecipe = new RecipeBuilder()
            .WithTitle("Original Recipe")
            .WithIngredients(ingredient1, ingredient2)
            .WithCategories(category1)
            .Build();

        Recipe savedRecipe = await recipeRepository.Create(originalRecipe);

        // Detach the savedRecipe from the DbContext to stop it from being tracked
        dbContext.Entry(savedRecipe).State = EntityState.Detached;

        // Update the recipe title, but keep the same relationships (ingredients and categories)
        savedRecipe.Title = "Updated Recipe Title";
        #endregion

        #region Act
        await recipeRepository.Update(savedRecipe, savedRecipe.ID);

        // Retrieve the updated recipe
        Recipe? updatedRecipe = await recipeRepository.GetOneWithAllDetails(savedRecipe.ID);
        #endregion

        #region Assert
        Assert.NotNull(updatedRecipe);
        Assert.Equal("Updated Recipe Title", updatedRecipe!.Title);

        // Check that the relationships are still intact (ingredients and categories)
        Assert.Equal(2, updatedRecipe.Ingredients!.Count);
        Assert.Contains(updatedRecipe.Ingredients, i => i.Name == "Tomato");
        Assert.Contains(updatedRecipe.Ingredients, i => i.Name == "Garlic");

        Assert.Single(updatedRecipe.Categories!);
        Assert.Contains(updatedRecipe.Categories!, c => c.Name == "Breakfast");

        // Assert that the category relationship has not changed (category1 should still be linked)
        #endregion
    }

    [Fact]
    public async Task UpdateRecipe_ReplacesOldIngredientsAndCategoriesWithNewSet()
    {
        var (recipeRepository, dbContext) = CreateScope();

        #region Arrange
        // Define initial ingredients and categories
        var initialIngredients = IngredientBuilder.BuildMany(2);

        var initialCategory = new Category { Name = "Breakfast" };

        // Define new ingredients and categories to replace the initial set
        var newIngredients = IngredientBuilder.BuildMany(2);

        var newCategory = new Category { Name = "Lunch" };

        // Add initial and new ingredients and categories to the DbContext
        dbContext.Ingredients.AddRange(initialIngredients.Concat(newIngredients));
        dbContext.Categories.AddRange(initialCategory, newCategory);
        await dbContext.SaveChangesAsync();

        // Create and save the original recipe with the initial set of ingredients and category
        var originalRecipe = new RecipeBuilder()
            .WithTitle("Original Recipe")
            .WithIngredients(initialIngredients.ToArray())
            .WithCategories(initialCategory)
            .Build();

        Recipe savedRecipe = await recipeRepository.Create(originalRecipe);

        // Detach the savedRecipe from the DbContext to stop it from being tracked
        dbContext.Entry(savedRecipe).State = EntityState.Detached;

        // Update the recipe with a new title, and replace ingredients and category
        savedRecipe.Title = "Updated Recipe Title";
        savedRecipe.Ingredients = newIngredients;  // Assign the new set of ingredients
        savedRecipe.Categories = new List<Category> { newCategory };  // Assign the new category
        #endregion

        #region Act
        await recipeRepository.Update(savedRecipe, savedRecipe.ID);

        // Retrieve the updated recipe with its ingredients and categories
        Recipe? updatedRecipe = await recipeRepository.GetOneWithAllDetails(savedRecipe.ID);
        #endregion

        #region Assert
        Assert.NotNull(updatedRecipe);
        Assert.Equal("Updated Recipe Title", updatedRecipe!.Title);

        // Verify the new set of ingredients is associated with the recipe
        Assert.Equal(newIngredients.Count, updatedRecipe.Ingredients!.Count);
        foreach (var newIngredient in newIngredients)
        {
            Assert.Contains(updatedRecipe.Ingredients, i => i.Name == newIngredient.Name);
        }

        // Ensure the old ingredients are no longer linked to the recipe
        foreach (var initialIngredient in initialIngredients)
        {
            Assert.DoesNotContain(updatedRecipe.Ingredients, i => i.Name == initialIngredient.Name);
        }

        // Verify the new category is associated with the recipe
        Assert.Single(updatedRecipe.Categories!);
        Assert.Contains(updatedRecipe.Categories!, c => c.Name == newCategory.Name);

        // Ensure the old category is no longer linked to the recipe
        Assert.DoesNotContain(updatedRecipe.Categories!, c => c.Name == initialCategory.Name);
        #endregion
    }

    [Fact]
    public async Task UpdateRecipe_AddsNewIngredientsAndCategoriesToExistingOnes()
    {
        var (recipeRepository, dbContext) = CreateScope();

        #region Arrange
        // Initial set of ingredients and categories
        var initialIngredients = IngredientBuilder.BuildMany(2);

        var initialCategory = new Category { Name = "Breakfast" };

        // New ingredients and categories to be added (not replacing, but adding to the existing ones)
        var additionalIngredients = IngredientBuilder.BuildMany(2);

        var additionalCategory = new Category { Name = "Lunch" };

        // Add both the initial and additional ingredients and categories to the DbContext
        dbContext.Ingredients.AddRange(initialIngredients.Concat(additionalIngredients));
        dbContext.Categories.AddRange(initialCategory, additionalCategory);
        await dbContext.SaveChangesAsync();

        // Create and save the original recipe with the initial set of ingredients and categories
        var originalRecipe = new RecipeBuilder()
            .WithTitle("Original Recipe")
            .WithIngredients(initialIngredients.ToArray())
            .WithCategories(initialCategory)
            .Build();

        Recipe savedRecipe = await recipeRepository.Create(originalRecipe);

        // Detach the savedRecipe from the DbContext to stop it from being tracked
        dbContext.Entry(savedRecipe).State = EntityState.Detached;

        // Update the recipe title and add the new ingredients and categories, while keeping the old ones
        savedRecipe.Title = "Updated Recipe Title";
        savedRecipe.Ingredients.AddRange(additionalIngredients);  // Add new ingredients to the existing ones
        savedRecipe.Categories.Add(additionalCategory);  // Add new category to the existing ones
        #endregion

        #region Act
        await recipeRepository.Update(savedRecipe, savedRecipe.ID);

        // Retrieve the updated recipe with details (including ingredients and categories)
        Recipe? updatedRecipe = await recipeRepository.GetOneWithAllDetails(savedRecipe.ID);
        #endregion

        #region Assert
        Assert.NotNull(updatedRecipe);
        Assert.Equal("Updated Recipe Title", updatedRecipe!.Title);

        // Verify that both the initial and additional ingredients are associated with the recipe
        var expectedIngredients = initialIngredients.Concat(additionalIngredients).ToList();
        Assert.Equal(expectedIngredients.Count, updatedRecipe.Ingredients!.Count);
        foreach (var ingredient in expectedIngredients)
        {
            Assert.Contains(updatedRecipe.Ingredients, i => i.Name == ingredient.Name);
        }

        // Verify that both the initial and additional categories are associated with the recipe
        var expectedCategories = new List<Category> { initialCategory, additionalCategory };
        Assert.Equal(expectedCategories.Count, updatedRecipe.Categories!.Count);
        foreach (var category in expectedCategories)
        {
            Assert.Contains(updatedRecipe.Categories!, c => c.Name == category.Name);
        }
        #endregion
    }



}
