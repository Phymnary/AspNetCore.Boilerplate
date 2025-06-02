namespace AspNetCore.Boilerplate.Api;

public class AspErrorMessage
{
    public required AspErrorDto Error { get; init; }
}

public class AspErrorDto
{
    public required string Message { get; init; }

    public required string? Details { get; init; }

    public IEnumerable<AspValidationErrorMessageDto>? ValidationErrors { get; init; }
}

public class AspValidationErrorMessageDto
{
    public required string Property { get; init; }

    public required string Message { get; init; }
}
