using Bistrosoft.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Bistrosoft.Infrastructure.Tests.Repositories;

public abstract class RepositoryTestBase : IDisposable
{
    protected ApplicationDbContext Context { get; }

    protected RepositoryTestBase()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new ApplicationDbContext(options);
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}
