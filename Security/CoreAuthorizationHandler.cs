using EnergyScanApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EnergyScanApi.Security
{
    public class CoreAuthorizationHandler : AuthorizationHandler<CoreRequirement>
    {
        private readonly AppDb Db;

        public CoreAuthorizationHandler(IConfiguration configuration)
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseMySql(configuration["ConnectionStrings:Default"], ServerVersion.AutoDetect(configuration["ConnectionStrings:Default"]));
            Db = new AppDb(optionsBuilder.Options, configuration);
        }
        public bool HasPermission(CoreRequirement requirement, ClaimsPrincipal user)
        {
            AuthorizationHandlerContext c = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            HandleRequirementAsync(c, requirement);
            return c.HasSucceeded;

        }

        // Check whether a user has a permission relation entry in the database for a particular context
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CoreRequirement requirement)
        {
            // Ignore UserClaims, just get the user id to check the database for a permission entry
            Claim CoreClaim = context.User.FindFirst(c => c.Type == CoreAuthorizationConstants.CoreClaimType);

            if (CoreClaim != null) //User Claims (a permission or whatever), we need to check if this is legit or forbidden
            {
                if (MeetPolicy(CoreClaim.Value, requirement.PolicyName))
                {
                    context.Succeed(requirement);
                } else
                {
                    context.Fail();
                }
            }
            return Task.CompletedTask;
        }
        private bool MeetPolicy(string userId, string policyName)
        {
            User u = Db.Users.Find(userId);
            Policy p = Db.Policies.Where(i => i.Name.Equals(policyName)).FirstOrDefault();
            UserPolicy up = Db.UserPolicies.Where(i => i.UserId.Equals(u.Id) && i.PolicyId.Equals(p.Id)).FirstOrDefault();
            if (up == null)
            {
                return false;
            }
            return true;

        }

    }
}
