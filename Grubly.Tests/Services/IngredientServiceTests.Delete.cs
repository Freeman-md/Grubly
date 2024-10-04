using System;
using Grubly.Interfaces.Repositories;
using Grubly.Services;
using Moq;

namespace Grubly.Tests.Services;

public partial class IngredientServiceTests
{
    [Fact]
    public async Task DeleteIngredient_CallsRepositoryDeleteMethodSuccessfully()
    {
        // Arrange
        var mockRepository = new Mock<IIngredientRepository>();
        var service = new IngredientService(mockRepository.Object);
        var ingredientId = 1;

        // Act
        await service.DeleteIngredient(ingredientId);

        // Assert
        mockRepository.Verify(repo => repo.Delete(ingredientId), Times.Once,
            "The Delete method should be called exactly once with the correct ingredient ID.");
    }

    [Fact]
    public async Task DeleteIngredient_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var mockRepository = new Mock<IIngredientRepository>();
        var service = new IngredientService(mockRepository.Object);
        var ingredientId = 1;

        mockRepository.Setup(repo => repo.Delete(ingredientId))
                      .ThrowsAsync(new KeyNotFoundException());

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.DeleteIngredient(ingredientId));
    }


}
