using Microsoft.AspNetCore.Authorization;

namespace EnergyScanApi.Security
{
    public class CoreRequirement : IAuthorizationRequirement
    {
        public string PolicyName { get; private set; }

        public CoreRequirement(string policyname) { PolicyName = policyname; }
    }
}
