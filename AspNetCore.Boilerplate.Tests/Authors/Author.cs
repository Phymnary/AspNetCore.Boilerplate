using AspNetCore.Boilerplate.Domain;

namespace AspNetCore.Boilerplate.Authors;

public class Author : Entity<int>
{
    public required string Name { get; set; }
}
