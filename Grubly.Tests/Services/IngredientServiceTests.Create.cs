using System;
using Grubly.Data;
using Grubly.Interfaces.Services;
using Grubly.Tests.Unit.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Grubly.Tests.Services;

public partial class IngredientServiceTests : IClassFixture<TestFixture>
{
    private readonly ServiceProvider _serviceProvider;

    public IngredientServiceTests(TestFixture fixture) {
        _serviceProvider = fixture.ServiceProvider;
    }

    private (IIngredientService ingredientService, GrublyContext dbContext) CreateScope() {
        var scope = _serviceProvider.CreateScope();
        var scopedServices = scope.ServiceProvider;

        var ingredientService = scopedServices.GetRequiredService<IIngredientService>();
        var dbContext = scopedServices.GetRequiredService<GrublyContext>();

        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        return (ingredientService, dbContext);
    }
}
