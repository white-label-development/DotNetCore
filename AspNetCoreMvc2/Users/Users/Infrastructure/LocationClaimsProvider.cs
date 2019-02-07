using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Users.Infrastructure
{
    public class LocationClaimsProvider : IClaimsTransformation
    {
        // This class simulates a system such as a central HR database, which would be the authoritative source of location information about staff
        //registered in startup: services.AddSingleton<IClaimsTransformation,LocationClaimsProvider>()


        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal != null
                && !principal.HasClaim(c => c.Type == ClaimTypes.PostalCode))
            {
                ClaimsIdentity identity = principal.Identity as ClaimsIdentity;
                if (identity != null && identity.IsAuthenticated
                                     && identity.Name != null)
                {
                    // I am logged in as admin for this run through, so add the DC claims there.
                    if (identity.Name.ToLower() == "admin")
                    {
                        identity.AddClaims(new Claim[] {
                            CreateClaim(ClaimTypes.PostalCode, "DC 20500"),
                            CreateClaim(ClaimTypes.StateOrProvince, "DC")
                        });
                    }
                    else
                    {
                        identity.AddClaims(new Claim[] {
                            CreateClaim(ClaimTypes.PostalCode, "NY 10036"),
                            CreateClaim(ClaimTypes.StateOrProvince, "NY")
                        });
                    }
                }
            }
            return Task.FromResult(principal);
        }

        private static Claim CreateClaim(string type, string value) =>
            new Claim(type, value, ClaimValueTypes.String, "RemoteClaims");
    }
}
