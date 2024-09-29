using System;
using Grubly.Data;
using Grubly.Repositories;
using Grubly.Tests.Unit.Fixtures;

namespace Grubly.Tests.Unit.Repository;

public partial class CategoryRepositoryTests : IClassFixture<TestFixtureBase>
{
    private readonly GrublyContext _dbContext;
    private readonly CategoryRepository _categoryRepository;
    public CategoryRepositoryTests(TestFixtureBase fixture) {
        _dbContext = fixture.DbContext;
        _categoryRepository = new CategoryRepository(_dbContext);

        fixture.ResetDatabase().Wait();
    }
}
