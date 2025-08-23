namespace Gallery.Models.Image;

public class CreateImageViewModel
{
    public required string Name { get; init; }
    public required string Year { get; init; }
    public required IFormFile ImageFile { get; init; }
}
