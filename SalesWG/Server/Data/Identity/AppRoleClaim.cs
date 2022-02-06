using Microsoft.AspNetCore.Identity;

namespace SalesWG.Server.Data.Identity
{
    public class AppRoleClaim : IdentityRoleClaim<string>
    {
        public string Description { get; set; }
        public string Group { get; set; }
        public virtual AppRole Role { get; set; }
    }
}
