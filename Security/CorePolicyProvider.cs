using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnergyScanApi.Security
{
    public class CorePolicyProvider : IAuthorizationPolicyProvider
    {
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; set; }
        public CorePolicyProvider(IOptions<AuthorizationOptions> options)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return FallbackPolicyProvider.GetDefaultPolicyAsync();
        }
        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            return FallbackPolicyProvider.GetFallbackPolicyAsync();
        }
        public Task<AuthorizationPolicy> GetFPolicyAsync()
        {
            return FallbackPolicyProvider.GetFallbackPolicyAsync();
        }
        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            List<string> p = new List<string>();
            //Core policies
            p.AddRange(new CorePolicys().GetPolicies());

            //Policy Build
            if (p.Contains(policyName))
            {
                AuthorizationPolicyBuilder policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new CoreRequirement(policyName));
                return Task.FromResult(policy.Build());
            }
            //Core policies are registered. here is room for other custom policy integrations

            //If no other custom policy is registered, 
            //try a fallback (=> resulting in null -> sending a 'AuthorizationPolicy not found' expeption back to client
            return FallbackPolicyProvider.GetPolicyAsync(policyName);
        }
    }
}
