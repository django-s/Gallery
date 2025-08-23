namespace Gallery.Models.Home;

public class HomeViewModel
{
    public required IReadOnlyList<Database.Image> Pictures { get; init; }
}
