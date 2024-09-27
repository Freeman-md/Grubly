using System;
using Grubly.Data;
using Grubly.Interfaces.Repositories;
using Grubly.Models;

namespace Grubly.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly GrublyContext _grublycontext;

    public RecipeRepository(GrublyContext grublyContext) {
        _grublycontext = grublyContext;
    }

    public Task<Recipe> Create(Recipe recipe)
    {
        throw new NotImplementedException();
    }

    public Task Delete(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Recipe>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<Recipe?> GetOne(int id) {
        throw new NotImplementedException();
    }

    public Task<Recipe?> GetOne(string title) {
        throw new NotImplementedException();
    }

    public Task<Recipe?> GetOneWithAllDetails(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Recipe?> GetOneWithAllDetails(string title)
    {
        throw new NotImplementedException();
    }

    public Task<Recipe> Update(Recipe recipe, int id)
    {
        throw new NotImplementedException();
    }
}
