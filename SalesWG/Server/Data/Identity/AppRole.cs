using Microsoft.AspNetCore.Identity;

namespace SalesWG.Server.Data.Identity
{
    public class AppRole : IdentityRole
    {
        public string Description { get; set; }
        public string LastName { get; set; }
        public virtual ICollection<AppRoleClaim> RoleClaims { get; set; }

        public AppRole() : base()
        {
            RoleClaims = new HashSet<AppRoleClaim>();
        }

        public AppRole(string roleName, string roleDescription = null) : base(roleName)
        {
            RoleClaims = new HashSet<AppRoleClaim>();
            Description = roleDescription;
        }
    }
}
