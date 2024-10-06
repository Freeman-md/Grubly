using System;
using Grubly.Interfaces.Repositories;
using Grubly.Models;
using Grubly.Repositories;
using Grubly.Services;
using Grubly.Tests.Unit.Builders;
using Moq;

namespace Grubly.Tests.Services;

public partial class CategoryServiceTests
{
    [Fact]
    public async Task DeleteCategory_CallsRepositoryDeleteMethodSuccessfully()
    {
        #region Arrange
        Category category = new CategoryBuilder().Build();

        _mockCategoryRepository.Setup(repo => repo.GetOne(It.IsAny<int>())).ReturnsAsync(category);
        #endregion


        #region Act
        await _categoryService.DeleteCategory(category.ID);
        #endregion

        #region Assert
        _mockCategoryRepository.Verify(repo => repo.Delete(category.ID), Times.Once,
        "The Delete method should be called exactly once with the correct category ID.");
        #endregion
    }

    [Fact]
    public async Task DeleteCategory_ThatDoesNotExist_ThrowsKeyNotFoundException()
    {
        #region Arrange
        _mockCategoryRepository.Setup(repo => repo.GetOne(It.IsAny<int>())).ReturnsAsync((Category)null);
        #endregion

        #region Act -> Assert 
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _categoryService.DeleteCategory(2));

        _mockCategoryRepository.Verify(repo => repo.GetOne(It.IsAny<int>()), Times.Once);
        #endregion
    }


}
