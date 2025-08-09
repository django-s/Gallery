namespace Gallery.Models.Account;

public sealed record ChangePasswordViewModel
{
    public required string OldPassword { get; init; }
    public required string NewPassword { get; init; }
}
