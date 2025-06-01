using AspNetCore.Boilerplate.Domain;

namespace AspNetCore.Boilerplate.Categories;

public class Category : Entity<int>
{
    public required string Name { get; set; }
}
