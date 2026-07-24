namespace Geekspace.ViewModels
{
    // Keeps the management view decoupled from IdentityUser.
    public class UserListItem
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = "User";
    }
}
