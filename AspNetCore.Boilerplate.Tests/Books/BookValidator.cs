using FluentValidation;

namespace AspNetCore.Boilerplate.Books;

[Dependency(Lifetime.Singleton)]
public class BookValidator : AbstractValidator<Book>
{
    public BookValidator()
    {
        RuleFor(book => book.Name).NotEmpty();
    }
}
