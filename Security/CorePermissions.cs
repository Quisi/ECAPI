
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Security.Principal;

namespace EnergyScanApi.Security
{
    public class CorePermissions : ControllerBase
    {
        private readonly CoreAuthorizationHandler coreAuthorizationHandler;

        public CorePermissions(IServiceProvider provider, IAuthorizationHandler coreAuthHandler)
        {
            coreAuthorizationHandler = (CoreAuthorizationHandler)coreAuthHandler;
        }
        public bool HasPermission(string CorePolicy, IIdentity User)
        {
            CoreRequirement corereq = new CoreRequirement(CorePolicy);
            ClaimsPrincipal coreuser = new ClaimsPrincipal(User);
            bool granted = coreAuthorizationHandler.HasPermission(corereq, coreuser);
            return granted;
        }
    }
}
