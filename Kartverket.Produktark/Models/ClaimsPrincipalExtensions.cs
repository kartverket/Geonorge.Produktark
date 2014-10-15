using System.Collections.Generic;
using System.Security.Claims;

namespace Kartverket.Produktark.Models
{
    public static class ClaimsPrincipalExtensions
    {
        public static string Uid(this ClaimsPrincipal principal)
        {
            return GetClaimValue(principal, "uid");
        }

        public static string Username(this ClaimsPrincipal principal)
        {
            return GetClaimValue(principal, "username");
        }

        public static string Organization(this ClaimsPrincipal principal)
        {
            return GetClaimValue(principal, "organization");
        }

        public static List<string> Roles(this ClaimsPrincipal principal)
        {
            var roles = new List<string>();

            IEnumerable<Claim> claims = principal.FindAll("role");
            if (claims != null)
            {
                foreach (var claim in claims)
                {
                    if (!string.IsNullOrWhiteSpace(claim.Value))
                    {
                        roles.Add(claim.Value);
                    }
                }    
            }

            return roles;
        }

        public static string Name(this ClaimsPrincipal principal)
        {
            return GetClaimValue(principal, "eduPersonPrincipalName");
        }

        public static string Email(this ClaimsPrincipal principal)
        {
            return GetClaimValue(principal, "email");
        }

        private static string GetClaimValue(this ClaimsPrincipal principal, string claimType)
        {
            Claim claim = principal.FindFirst(claimType);
            return claim != null ? claim.Value : null;
        }
    }
}