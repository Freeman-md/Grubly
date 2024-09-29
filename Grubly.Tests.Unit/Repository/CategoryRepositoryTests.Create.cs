using System;
using System.ComponentModel.DataAnnotations;
using Grubly.Data;
using Grubly.Models;
using Grubly.Repositories;
using Grubly.Tests.Unit.Builders;
using Grubly.Tests.Unit.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace Grubly.Tests.Unit.Repository;

public partial class CategoryRepositoryTests : IClassFixture<TestFixtureBase>
{
    private readonly GrublyContext _dbContext;
    private readonly CategoryRepository _categoryRepository;
    public CategoryRepositoryTests(TestFixtureBase fixture) {
        _dbContext = fixture.DbContext;
        _categoryRepository = new CategoryRepository(_dbContext);

        fixture.ResetDatabase().Wait();
    }

    [Fact]
    public async Task CreateCategory_ValidInput_AddsCategoryToDatabase()
    {
        #region Arrange
        Category unSavedCategory = new CategoryBuilder().Build();
        #endregion

        #region Act
        Category savedCategory = await _categoryRepository.Create(unSavedCategory);
        #endregion

        #region Assert
        Assert.NotNull(savedCategory);
        Assert.True(savedCategory.ID > 0, "The Category ID should be greater than 0 after saving to the database.");
        Assert.Equal(unSavedCategory.Name, savedCategory.Name);
        #endregion
    }

    [Fact]
    public async Task CreateCategory_NullInput_ThrowsArgumentNullException()
    {
        #region Arrange
        Category? nullCategory = null;
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _categoryRepository.Create(nullCategory!));
        #endregion
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task CreateCategory_InvalidInputs_ThrowsValidationException(string title)
    {
        #region Arrange
        Category unSavedCategory = new Category { Name = title};
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _categoryRepository.Create(unSavedCategory));
        #endregion
    }

    [Fact]
    public async Task CreateCategory_DuplicateEntity_ThrowsArgumentException()
    {
        #region Arrange
        Category unSavedCategory = new CategoryBuilder().Build();
        Category sameCategory = unSavedCategory;
        #endregion

        #region Act
        await _categoryRepository.Create(unSavedCategory);
        #endregion

        #region Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _categoryRepository.Create(sameCategory));
        #endregion
    }

    [Fact]
    public async Task CreateCategory_WithRelations_EnsuresCorrectForeignKeysAndSavesRelatedEntities()
    {
        #region Arrange
        const int NUMBER_OF_CREATED_RECIPES = 3;
        List<Recipe> recipes = RecipeBuilder.BuildMany(NUMBER_OF_CREATED_RECIPES);

        _dbContext.Recipes.AddRange(recipes);
        await _dbContext.SaveChangesAsync();

        Category unSavedCategory = new CategoryBuilder()
                                    .WithName("Tomatoes")
                                    .WithRecipes(recipes.ToArray())
                                    .Build();
        #endregion

        #region Act
        Category savedCategory = await _categoryRepository.Create(unSavedCategory);
        #endregion

        #region Assert
        Assert.True(savedCategory.ID > 0, "The Category ID should be greater than 0 after saving to the database.");

        // get model directly from db using repository to ensure relations were saved
        Category? retrievedCategory = await _categoryRepository.GetOneWithAllDetails(savedCategory.ID);
        Assert.NotNull(retrievedCategory);
        Assert.Equal(unSavedCategory.Name, retrievedCategory!.Name);

        Assert.Equal(recipes.Count, retrievedCategory.Recipes!.Count);
        foreach (var recipe in recipes)
        {
            Assert.Contains(retrievedCategory.Recipes, r => r.Title == recipe.Title && r.Description == recipe.Description);
        }
        #endregion
    }


    [Fact]
    public async Task CreateCategory_InvalidForeignKey_ThrowsDbUpdateException()
    {
        #region Arrange
        Recipe recipe = new RecipeBuilder().Build();
        recipe.ID = 3892; // invalid ID

        Category unSavedCategory = new CategoryBuilder()
            .WithName("Invalid Category")
            .WithRecipes(recipe)
            .Build();
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => _categoryRepository.Create(unSavedCategory));
        #endregion
    }
}
