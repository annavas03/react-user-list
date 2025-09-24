namespace app.Server.Models.Users
{
    public class UserItemModel
    {
        public long Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Image { get; set; } = null!;
        public string LoginType { get; set; } = null!;
        public string[] Roles { get; set; } = null!;
    }
}
