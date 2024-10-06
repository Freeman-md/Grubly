using System;
using Grubly.Interfaces.Repositories;
using Grubly.Models;
using Grubly.Repositories;
using Grubly.Services;
using Moq;

namespace Grubly.Tests.Services;

public partial class CategoryServiceTests
{
    [Fact]
    public async Task DeleteCategory_CallsRepositoryDeleteMethodSuccessfully()
    {
        #region Arrange
        var categoryId = 1;
        #endregion


        #region Act
        await _categoryService.DeleteCategory(categoryId);
        #endregion

        #region Assert
        _mockCategoryRepository.Verify(repo => repo.Delete(categoryId), Times.Once,
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
