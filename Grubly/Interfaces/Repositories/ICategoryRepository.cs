using System;
using Grubly.Models;

namespace Grubly.Interfaces.Repositories;

public interface ICategoryRepository
{
    public Task<IReadOnlyList<Category>> GetAll();
    public Task<Category> GetOne(int id);
    public Task<Category> GetOne(string name);
    public Task<Category> Create(Category category);
    public Task<Category> Update(Category category, int id);
    public Task Delete(int id);
}
