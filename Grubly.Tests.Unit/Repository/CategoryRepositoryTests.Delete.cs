using System;
using Grubly.Models;
using Grubly.Tests.Unit.Builders;
using Microsoft.EntityFrameworkCore;

namespace Grubly.Tests.Unit.Repository;

public partial class CategoryRepositoryTests
{
[Fact]
    public async Task DeleteCategory_ValidId_RemovesCategoryFromDatabase()
    {
        #region Arrange
        Category savedCategory = await _categoryRepository.Create(new CategoryBuilder().Build());
        #endregion

        #region Act
        await _categoryRepository.Delete(savedCategory.ID);
        #endregion

        #region Assert
        Category? nullCategory = await _categoryRepository.GetOne(savedCategory.ID);
        Assert.Null(nullCategory);
        #endregion
    }

    [Fact]
    public async Task DeleteCategory_InvalidId_ThrowsNotFoundException()
    {
        //TODO: Create a NotFoundException Class in main project and use here

        #region Arrange
        Category savedCategory = await _categoryRepository.Create(new CategoryBuilder().Build());
        await _categoryRepository.Delete(savedCategory.ID);
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await _categoryRepository.Delete(savedCategory.ID));
        #endregion
    }

    [Fact]
    public async Task DeleteCategory_WithManyToManyRelationships_RemovesLinksButKeepsEntities()
    {
        #region Arrange
        const int NUMBER_OF_CREATED_RECIPES = 3;
        List<Recipe> recipes = RecipeBuilder.BuildMany(NUMBER_OF_CREATED_RECIPES);

        _dbContext.Recipes.AddRange(recipes);
        await _dbContext.SaveChangesAsync();

        Category savedCategory = await _categoryRepository.Create(
                                                            new CategoryBuilder()
                                                            .WithRecipes(recipes.ToArray())
                                                            .Build()
                                                            );
        #endregion

        #region Act
        await _categoryRepository.Delete(savedCategory.ID);
        #endregion

        #region Assert
        Category? nullCategory = await _categoryRepository.GetOne(savedCategory.ID);
        Assert.Null(nullCategory);

        foreach (var recipe in recipes)
        {
            Recipe? existingRecipe = await _dbContext.Recipes.FirstOrDefaultAsync((recipe) => recipe.Title == recipe.Title);
            Assert.NotNull(existingRecipe);

            Assert.DoesNotContain(existingRecipe.Categories, (recipe) => recipe.ID == savedCategory.ID);
        }
        #endregion
    }
}
