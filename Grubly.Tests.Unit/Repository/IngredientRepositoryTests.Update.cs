using System;
using System.ComponentModel.DataAnnotations;
using Grubly.Models;
using Grubly.Tests.Unit.Builders;

namespace Grubly.Tests.Unit.Repository;

public partial class IngredientRepositoryTests
{
    [Theory]
    [InlineData("Updated Name", "Updated Description")]
    public async Task UpdateIngredient_ValidEntity_UpdatesIngredientInDatabase(string name, string description)
    {
        #region Arrange
        Ingredient savedIngredient = await _ingredientRepository.Create(new IngredientBuilder().Build());

        savedIngredient.Name = name;
        savedIngredient.Description = description;
        #endregion

        #region Act
        Ingredient updatedIngredient = await _ingredientRepository.Update(savedIngredient, savedIngredient.ID);
        #endregion

        #region Assert
        Ingredient? retrievedIngredient = await _ingredientRepository.GetOne(updatedIngredient.ID);

        Assert.NotNull(retrievedIngredient);

        Assert.True(updatedIngredient.ID > 0, "The Ingredient ID should be greater than 0 after saving to the database.");
        Assert.Equal(updatedIngredient.Name, retrievedIngredient!.Name);
        Assert.Equal(updatedIngredient.Description, retrievedIngredient.Description);
        #endregion
    }

    [Theory]
    [InlineData(null, "Valid Description")]
    [InlineData("", "Valid Description")]
    public async Task UpdateIngredient_InvalidInputs_ThrowsValidationException(string name, string description)
    {
        #region Arrange
        Ingredient savedIngredient = await _ingredientRepository.Create(new IngredientBuilder().Build());
        savedIngredient.Name = name;
        savedIngredient.Description = description;
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _ingredientRepository.Update(savedIngredient, savedIngredient.ID));
        #endregion
    }

    [Fact]
    public async Task UpdateIngredient_InvalidId_ThrowsNotFoundException()
    {
        //TODO: Create a NotFoundException Class in main project and use here
        #region Arrange
        const int RANDOM_ID = 82923;
        Ingredient savedIngredient = await _ingredientRepository.Create(new IngredientBuilder().Build());
        #endregion

        #region Assert
        await Assert.ThrowsAsync<Exception>(async () => await _ingredientRepository.Update(savedIngredient, RANDOM_ID));
        #endregion
    }

    [Fact]
    public async Task UpdateIngredient_DatabaseIntegrity_MaintainsRelationships()
    {
        #region Arrange
        const int NUMBER_OF_CREATED_RECIPES = 3;
        const string UPDATED_NAME = "Updated Ingredient Name";
        List<Recipe> recipes = RecipeBuilder.BuildMany(NUMBER_OF_CREATED_RECIPES);

        _dbContext.Recipes.AddRange(recipes);
        await _dbContext.SaveChangesAsync();

        Ingredient originalIngredient = new IngredientBuilder()
            .WithName("Original Ingredient")
            .WithRecipes(recipes.ToArray())
            .Build();

        Ingredient savedIngredient = await _ingredientRepository.Create(originalIngredient);

        savedIngredient.Name = UPDATED_NAME;
        #endregion

        #region Act
        await _ingredientRepository.Update(savedIngredient, savedIngredient.ID);

        Ingredient? updatedIngredient = await _ingredientRepository.GetOneWithAllDetails(savedIngredient.ID);
        #endregion

        #region Assert
        Assert.NotNull(updatedIngredient);
        Assert.True(updatedIngredient.ID > 0, "The Ingredient ID should be greater than 0 after saving to the database.");
        Assert.Equal(UPDATED_NAME, updatedIngredient!.Name);

        Assert.Equal(recipes.Count, updatedIngredient.Recipes!.Count);
        #endregion
    }
}
