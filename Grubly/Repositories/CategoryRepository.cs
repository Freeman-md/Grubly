using System;
using Grubly.Data;
using Grubly.Interfaces.Repositories;
using Grubly.Models;

namespace Grubly.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly GrublyContext _grublyContext;

    public CategoryRepository(GrublyContext grublyContext)
    {
        _grublyContext = grublyContext;
    }

    public Task<Category> Create(Category category)
    {
        throw new NotImplementedException();
    }

    public Task Delete(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Category>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<Category> GetOne(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Category> GetOne(string name)
    {
        throw new NotImplementedException();
    }

    public Task<Category> GetOneWithAllDetails(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Category> GetOneWithAllDetails(string name)
    {
        throw new NotImplementedException();
    }

    public Task<Category> Update(Category category, int id)
    {
        throw new NotImplementedException();
    }
}
