using BlogApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Post> Posts => Set<Post>();
}
