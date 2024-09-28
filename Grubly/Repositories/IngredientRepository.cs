using System;
using Grubly.Data;
using Grubly.Interfaces.Repositories;
using Grubly.Models;

namespace Grubly.Repositories;

public class IngredientRepository : IIngredientRepository
{
    private readonly GrublyContext _grublyContext;

    public IngredientRepository(GrublyContext grublyContext) {
        _grublyContext = grublyContext;
    }

    public Task<Ingredient> Create(Ingredient ingredient)
    {
        throw new NotImplementedException();
    }

    public Task Delete(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Ingredient>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<Ingredient> GetOne(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Ingredient> GetOne(string name)
    {
        throw new NotImplementedException();
    }

    public Task<Ingredient> GetOneWithAllDetails(int id) {
        throw new NotImplementedException();
    }

    public Task<Ingredient> GetOneWithAllDetails(string name) {
        throw new NotImplementedException();
    }

    public Task<Ingredient> Update(Ingredient ingredient, int id)
    {
        throw new NotImplementedException();
    }
}
