
using Microsoft.AspNetCore.Identity;

namespace app.Server.Data.Entities.Identity
{
    public class UserEntity : IdentityUser<long>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string? Image { get; set; }

        public string LoginType { get; set; } = "Email";

        public bool IsManual { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
    }
}
