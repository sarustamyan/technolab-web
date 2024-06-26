﻿using System.Security.Claims;

namespace Technolab.OnlineLibrary.Web.Models
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUsername(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(Consts.Claim.Username);
        }

        public static string GetFirstName(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(Consts.Claim.FirstName);
        }

        public static string GetLastName(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(Consts.Claim.LastName);
        }

        public static string GetFullName(this ClaimsPrincipal principal)
        {
            return $"{principal.GetFirstName()} {principal.GetLastName()}";
        }
    }
}
