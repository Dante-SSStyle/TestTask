using Microsoft.EntityFrameworkCore;

namespace TestTask.Domain;

/// <summary>
/// Custom DbContext.
/// </summary>
public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<DataModel> DataModel { get; set; } = default!;
}