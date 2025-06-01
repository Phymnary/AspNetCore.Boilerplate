using AspNetCore.Boilerplate.Authors;
using AspNetCore.Boilerplate.Categories;
using AspNetCore.Boilerplate.Domain;

namespace AspNetCore.Boilerplate.Books;

public class Book : Entity<int>, IAuditable
{
    public required string Name { get; set; }

    public required ICollection<BookAuthor> Authors { get; init; }

    public Category? Category { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid? CreatedById { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedById { get; set; }
}
