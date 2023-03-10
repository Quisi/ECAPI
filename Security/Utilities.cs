using EnergyScanApi.Models;
using System.Linq;

namespace EnergyScanApi.Security
{
    public class Utilities
    {
        readonly AppDb Db;
        public Utilities(AppDb db)
        {
            Db = db;
        }
        public User GetCurrentUser(System.Security.Claims.ClaimsPrincipal principal)
        {
            System.Security.Claims.Claim c = principal.Claims.Where(i => i.Type.Equals("userId")).FirstOrDefault();
            if (c != null)
            {
                User u = Db.Users.Where(i => i.Id.Equals(c.Value)).FirstOrDefault();
                return u;
            }
            return new User() { Id = "-1" };
        }
        public string GetCurrentUserId(System.Security.Claims.ClaimsPrincipal principal)
        {
            return GetCurrentUser(principal).Id;
        }
    }
}
