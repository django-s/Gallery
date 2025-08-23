namespace Gallery.Database;

public sealed class Image
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Year { get; init; }
    public required string Url { get; init; }
    public required string FilePath { get; init; }
}
