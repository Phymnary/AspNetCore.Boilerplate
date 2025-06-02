using AspNetCore.Boilerplate.Domain;
using AspNetCore.Boilerplate.EntityFrameworkCore;

namespace AspNetCore.Boilerplate.Books;

public interface IBookRepository : IRepository<Book>;

[Dependency(Lifetime.Scoped)]
public class BookRepository(
    BookStoreDbContext context,
    EfRepositoryAddons addons,
    IRepositoryOptions<Book> options
) : EfRepository<BookStoreDbContext, Book>(context, addons, options), IBookRepository;
