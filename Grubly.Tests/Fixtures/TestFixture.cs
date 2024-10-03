using System;
using Grubly.Data;
using Grubly.Interfaces.Repositories;
using Grubly.Models;
using Grubly.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;
using Grubly.Interfaces.Services;
using Grubly.Services;

namespace Grubly.Tests.Unit.Fixtures;

public class TestFixture : IDisposable
{
    public ServiceProvider ServiceProvider { get; private set; }
    private readonly SqliteConnection _connection;


    public TestFixture()
    {
        var services = new ServiceCollection();

        // Use SQLite in-memory database for testing
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        services.AddDbContext<GrublyContext>(options =>
            options.UseSqlite(_connection));

        services.AddScoped<IIngredientRepository, IngredientRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();

        services.AddScoped<IIngredientService, IngredientService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IRecipeService, RecipeService>();

        ServiceProvider = services.BuildServiceProvider();

        // Ensure the database schema is created
        using (var scope = ServiceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<GrublyContext>();
            dbContext.Database.EnsureCreated();
        }
    }

    public void Dispose()
    {
        var dbContext = ServiceProvider.GetService<GrublyContext>();

        dbContext?.Database.EnsureDeleted();

        _connection.Close();
        _connection.Dispose();
    }

}
