namespace Gallery.Models.Image;

public class DeleteImagesViewModel
{
    public required IReadOnlyList<Database.Image> Images { get; init; }
    public required IReadOnlyList<bool> DoDelete { get; init; }
    public required IReadOnlyList<Guid> ImageIds { get; init; }
}
