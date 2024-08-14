namespace Yev.Bonobo.Models.Api;

public sealed class CreateRepoModel
{
    public string? Description { get; set; }
    public required string Name { get; set; }
    public required string Path { get; set; }
}