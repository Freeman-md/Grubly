using System;
using System.ComponentModel.DataAnnotations;
using Grubly.Data;
using Grubly.Interfaces.Repositories;
using Grubly.Models;
using Grubly.Repositories;
using Grubly.Tests.Unit.Builders;
using Grubly.Tests.Unit.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Grubly.Tests.Unit.Repository;

public partial class CategoryRepositoryTests : IClassFixture<TestFixture>
{
    private readonly ServiceProvider _serviceProvider;

    public CategoryRepositoryTests(TestFixture fixture)
    {
        _serviceProvider = fixture.ServiceProvider;
    }

    private (ICategoryRepository categoryRepository, GrublyContext dbContext) CreateScope()
    {
        var scope = _serviceProvider.CreateScope();
        var scopedServices = scope.ServiceProvider;

        var categoryRepository = scopedServices.GetRequiredService<ICategoryRepository>();
        var dbContext = scopedServices.GetRequiredService<GrublyContext>();

        return (categoryRepository, dbContext);
    }

    [Fact]
    public async Task CreateCategory_ValidInput_AddsCategoryToDatabase()
    {
        var (categoryRepository, dbContext) = CreateScope();

        #region Arrange
        Category unSavedCategory = new CategoryBuilder().Build();
        #endregion

        #region Act
        Category savedCategory = await categoryRepository.Create(unSavedCategory);
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
        var (categoryRepository, dbContext) = CreateScope();

        #region Arrange
        Category? nullCategory = null;
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => categoryRepository.Create(nullCategory!));
        #endregion
    }

    [Theory]
    [InlineData(null)]
    public async Task CreateCategory_InvalidInputs_ThrowsDbUpdateException(string title)
    {
        var (categoryRepository, dbContext) = CreateScope();

        #region Arrange
        Category unSavedCategory = new Category { Name = title };
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => categoryRepository.Create(unSavedCategory));
        #endregion
    }

    [Fact]
    public async Task CreateCategory_DuplicateEntity_ThrowsDbUpdateException()
    {
        var (categoryRepository, dbContext) = CreateScope();

        #region Arrange
        Category originalCategory = new CategoryBuilder().Build();
        Category duplicateCategory = new Category
        {
            Name = originalCategory.Name,
        };
        #endregion

        #region Act
        await categoryRepository.Create(originalCategory);
        #endregion

        #region Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => categoryRepository.Create(duplicateCategory));
        #endregion
    }

    [Fact]
    public async Task CreateCategory_InvalidForeignKey_ThrowsDbUpdateException()
    {
        var (categoryRepository, dbContext) = CreateScope();

        #region Arrange
        Recipe recipe = new RecipeBuilder().Build();
        recipe.ID = 3892; // invalid ID

        Category unSavedCategory = new CategoryBuilder()
            .WithName("Invalid Category")
            .WithRecipes(recipe)
            .Build();
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => categoryRepository.Create(unSavedCategory));
        #endregion
    }

    //  [Fact]
    // public async Task CreateCategory_WithRelations_EnsuresCorrectForeignKeysAndSavesRelatedEntities()
    // {
    //     var (categoryRepository, dbContext) = CreateScope();

    //     #region Arrange
    //     const int NUMBER_OF_CREATED_RECIPES = 3;
    //     List<Recipe> recipes = RecipeBuilder.BuildMany(NUMBER_OF_CREATED_RECIPES);

    //     dbContext.Recipes.AddRange(recipes);
    //     await dbContext.SaveChangesAsync();

    //     Category unSavedCategory = new CategoryBuilder()
    //                                 .WithName("Tomatoes")
    //                                 .WithRecipes(recipes.ToArray())
    //                                 .Build();
    //     #endregion

    //     #region Act
    //     Category savedCategory = await categoryRepository.Create(unSavedCategory);
    //     #endregion

    //     #region Assert
    //     Assert.True(savedCategory.ID > 0, "The Category ID should be greater than 0 after saving to the database.");

    //     // get model directly from db using repository to ensure relations were saved
    //     Category? retrievedCategory = await categoryRepository.GetOneWithAllDetails(savedCategory.ID);
    //     Assert.NotNull(retrievedCategory);
    //     Assert.Equal(unSavedCategory.Name, retrievedCategory!.Name);

    //     Assert.Equal(recipes.Count, retrievedCategory.Recipes!.Count);
    //     foreach (var recipe in recipes)
    //     {
    //         Assert.Contains(retrievedCategory.Recipes, r => r.Title == recipe.Title && r.Description == recipe.Description);
    //     }
    //     #endregion
    // }
}
