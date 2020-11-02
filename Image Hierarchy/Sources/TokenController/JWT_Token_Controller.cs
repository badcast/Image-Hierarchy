using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

using ImageHierarchy.Models;


namespace ImageHierarchy
{
    public class JWT_Token_Controller : Controller
    {
        public static string create_token(ClaimsIdentity identity)
        {
            DateTime now = DateTime.UtcNow;
            JwtSecurityToken payload = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME_TOKEN)),
                    signingCredentials: new SigningCredentials(AuthOptions.Get_Symmetric_SecurityKey(), SecurityAlgorithms.HmacSha256));
            string encodedJwt = new JwtSecurityTokenHandler().WriteToken(payload);
            return encodedJwt;
        }

        public static bool check_token(string jwtToken)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            try
            {
                handler.ValidateToken(jwtToken, new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateLifetime = true,

                    IssuerSigningKey = AuthOptions.Get_Symmetric_SecurityKey(),
                    
                    ValidateIssuer = true,
                    ValidIssuer = AuthOptions.ISSUER,

                    ValidAudience = AuthOptions.AUDIENCE,
                    ValidateAudience = true,

                }, out SecurityToken tokenResult);

                if(tokenResult != null)
                {
                    return true;
                }

            }
            catch (Exception ex) { }
            return false;
        }
    }
}
