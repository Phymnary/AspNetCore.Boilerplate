using AspNetCore.Boilerplate.Authors;
using AspNetCore.Boilerplate.Books;
using AspNetCore.Boilerplate.Categories;
using AspNetCore.Boilerplate.Domain;
using AspNetCore.Boilerplate.Domain.Auditing;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Boilerplate.EntityFrameworkCore;

public class BookStoreDbContext(DbContextOptions<BookStoreDbContext> options) : DbContext(options)
{
    public DbSet<Book> Books { get; init; }

    public DbSet<Author> Authors { get; init; }

    public DbSet<BookAuthor> BookAuthors { get; init; }

    public DbSet<Category> Categories { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        new ModelBuilderHelper(modelBuilder)
            .BuildEntity<Book>()
            .BuildEntity<Author>()
            .BuildEntity<BookAuthor>()
            .BuildEntity<Category>()
            .BuildEntity<EntityPropertyChange>();
    }
}
