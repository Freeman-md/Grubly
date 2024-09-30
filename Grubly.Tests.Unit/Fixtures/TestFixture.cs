using System;
using Grubly.Data;
using Grubly.Interfaces.Repositories;
using Grubly.Models;
using Grubly.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Grubly.Tests.Unit.Fixtures;

public class TestFixture : IDisposable
{
    public ServiceProvider ServiceProvider { get; private set; }


    public TestFixture()
    {
        var services = new ServiceCollection();

        // Use In-Memory Database for testing
        services.AddDbContext<GrublyContext>(options => options.UseInMemoryDatabase("GrublyDB"));

        services.AddScoped<IIngredientRepository, IngredientRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();

        ServiceProvider = services.BuildServiceProvider();
    }

    public void Dispose()
    {
        var dbContext = ServiceProvider.GetService<GrublyContext>();
        
        dbContext?.Database.EnsureDeleted();
    }

}
