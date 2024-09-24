using System;
using Grubly.Models;

namespace Grubly.Interfaces.Repositories;

public interface ICategoryRepository
{
    public Task<IReadOnlyList<Category>> GetAll();
    public Task<Category> GetById(int id);
    public Task<Category> Create(Category category);
    public Task<Category> Update(Category category, int id);
    public Task Delete(int id);
}
