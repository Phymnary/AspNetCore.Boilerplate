using AspNetCore.Boilerplate.Authors;
using AspNetCore.Boilerplate.Domain;

namespace AspNetCore.Boilerplate.Books;

public class BookAuthor : Entity<int>
{
    public required Book Book { get; init; }

    public required Author Author { get; init; }
}
