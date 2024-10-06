using System;
using Grubly.Interfaces.Repositories;
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
    public async Task DeleteCategory_RepositoryThrowsException_PropagatesException()
    {
        #region Arrange
        var categoryId = 1;

        _mockCategoryRepository.Setup(repo => repo.Delete(categoryId))
                      .ThrowsAsync(new KeyNotFoundException());
        #endregion

        #region Act -> Assert 
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _categoryService.DeleteCategory(categoryId));
        #endregion
    }


}
