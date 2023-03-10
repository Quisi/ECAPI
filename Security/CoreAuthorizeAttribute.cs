using Microsoft.AspNetCore.Authorization;

namespace EnergyScanApi.Security
{
    public class CoreAuthorizeAttribute : AuthorizeAttribute
    {
        private const string POLICY_PREFIX = CoreAuthorizationConstants.CoreAttributePrefix;

        public CoreAuthorizeAttribute(string policyname)
        {
            PolicyName = policyname;
        }

        // Get or set the property by manipulating the underlying Policy property
        public string PolicyName
        {
            get => Policy;
            set => Policy = $"{value}";
        }
    }
}
