using System;
using Grubly.Models;
using Grubly.Tests.Unit.Builders;

namespace Grubly.Tests.Unit.Repository;

public partial class IngredientRepositoryTests
{
    [Fact]
    public async Task GetIngredientById_ValidId_ReturnsCorrectIngredient()
    {
        #region Arrange
        Ingredient unSavedIngredient = new IngredientBuilder().Build();
        Ingredient savedIngredient = await _ingredientRepository.Create(unSavedIngredient);
        #endregion

        #region Act
        Ingredient? retrievedIngredient = await _ingredientRepository.GetOne(savedIngredient.ID);
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
        #region Arrange
        Ingredient unSavedIngredient = new IngredientBuilder().Build();
        Ingredient savedIngredient = await _ingredientRepository.Create(unSavedIngredient);
        #endregion

        #region Act
        Ingredient? retrievedIngredient = await _ingredientRepository.GetOne(savedIngredient.Name);
        #endregion

        #region Assert
        Assert.NotNull(retrievedIngredient);
        Assert.Equal(savedIngredient.Name, retrievedIngredient.Name);
        Assert.Equal(savedIngredient.Description, retrievedIngredient.Description);
        #endregion
    }

    [Fact]
    public async Task GetIngredientById_InvalidId_ReturnsNull()
    {
        #region Arrange
        Ingredient ingredient = new IngredientBuilder().Build();
        ingredient.ID = 893; // random ID
        #endregion

        #region Act
        Ingredient? nullIngredient = await _ingredientRepository.GetOne(ingredient.ID);
        #endregion

        #region Assert
        Assert.Null(nullIngredient);
        #endregion
    }

    [Fact]
    public async Task GetIngredientByName_InvalidName_ReturnsNull()
    {
        #region Arrange
        Ingredient ingredient = new IngredientBuilder().Build();
        #endregion

        #region Act
        Ingredient? nullIngredient = await _ingredientRepository.GetOne(ingredient.Name);
        #endregion

        #region Assert
        Assert.Null(nullIngredient);
        #endregion
    }

    [Fact]
    public async Task GetAllIngredients_ReturnsAllIngredients() {
        #region Arrange
            Ingredient[] unSavedIngredients = {
                new IngredientBuilder().WithName($"Tomato").Build(),
                new IngredientBuilder().WithName($"Cheese").Build(),
                new IngredientBuilder().WithName($"Raisin").Build()
            };

            foreach (Ingredient ingredient in unSavedIngredients) {
                await _ingredientRepository.Create(ingredient);
            }
        #endregion

        #region Act
            IReadOnlyList<Ingredient> savedIngredients = await _ingredientRepository.GetAll();
        #endregion

        #region Assert
            Assert.NotNull(savedIngredients);
            Assert.Equal(unSavedIngredients.Length, savedIngredients.Count);
            Assert.Contains(savedIngredients, (ingredient) => ingredient.Name.Equals(unSavedIngredients[0].Name));
        #endregion
    }

     [Fact]
    public async Task GetAllIngredients_EmptyDatabase_ReturnsEmptyList() {
        #region Act
            IReadOnlyList<Ingredient> noIngredients = await _ingredientRepository.GetAll();
        #endregion

        #region Assert
            Assert.NotNull(noIngredients);
            Assert.Empty(noIngredients);
        #endregion
    }
}
