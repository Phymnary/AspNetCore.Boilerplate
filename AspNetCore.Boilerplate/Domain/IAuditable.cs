namespace AspNetCore.Boilerplate.Domain;

public interface IAuditable
{
    DateTime CreatedAt { get; set; }

    Guid? CreatedById { get; set; }

    DateTime? UpdatedAt { get; set; }

    Guid? UpdatedById { get; set; }
}
