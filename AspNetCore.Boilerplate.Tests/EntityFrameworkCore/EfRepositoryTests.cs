using System.Text.Json;
using AspNetCore.Boilerplate.Api;
using AspNetCore.Boilerplate.Books;
using AspNetCore.Boilerplate.Categories;
using AspNetCore.Boilerplate.EntityFrameworkCore.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Boilerplate.EntityFrameworkCore;

public class EfRepositoryTests
{
    [Fact]
    public async Task track_changes_test()
    {
        var options = new DbContextOptionsBuilder<BookStoreDbContext>()
            .UseInMemoryDatabase(nameof(track_changes_test))
            .Options;
        var dbContext = new BookStoreDbContext(options);
        var repository = new BookRepository(
            dbContext,
            new EfRepositoryAddons(
                [
                    new AuditSaveChangesInterceptor<BookStoreDbContext>(
                        dbContext,
                        new HttpContextCurrentUser()
                    ),
                ]
            ),
            new BookRepositoryOptions(new BookValidator())
        );

        var book = await repository.InsertAsync(new Book { Name = "Harry Potter", Authors = [] });
        book.Name = "Harry Potter chapter 1";
        book.Category = new Category { Name = "Children Books" };
        await repository.UpdateAsync(book);

        book.Name = "Harry Potter ch. 1";
        await repository.UpdateAsync(book);


        var dto = await repository.ReadonlyQuery(entity => entity.Id == 1).ToListAsync();
    }
}
