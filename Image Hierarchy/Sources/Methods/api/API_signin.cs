using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Text.Json; // JSON

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.IdentityModel.Tokens;

using ImageHierarchy.Models;

namespace ImageHierarchy
{
    public partial class AccountController
    {
        async Task<IActionResult> extern_signin()
        {
            Mo_User_SignIn model;
            UserAccount uacc;
            PairValue<Mo_Result, UserAccount> backward;
            ClaimsIdentity claimsIdentity;
            List<Claim> claims;
            Mo_AuthResult frontend = new Mo_AuthResult();
            string reqJson = string.Empty;
            if((Request.ContentLength.HasValue && Request.ContentLength > 0 || Request.Body.CanRead))
            {
                using(var reader = new System.IO.StreamReader(Request.Body))
                    reqJson = await reader.ReadToEndAsync();
                if(!string.IsNullOrEmpty(reqJson))
                {
                    model = JsonSerializer.Deserialize<Mo_User_SignIn>(reqJson);
                    // go sign in
                    backward = await signin(model);
                    uacc = backward.second;
                    if(backward.first.result)
                    {
                        //complete
                        claims = new List<Claim>(2){
                            new Claim("username", uacc.username),
                            new Claim("role", uacc.role ?? "User")
                        };
                        claimsIdentity = new ClaimsIdentity(claims, "Token", "untype", "unroletype");
                        string token = JWT_Token_Controller.create_token(claimsIdentity);

                        frontend.access_token = token;
                        frontend.refresh_token = null;
                        frontend.result = true;
                        frontend.role = uacc.role;
                        frontend.loggedDate = uacc.lastLoggedDate;
                        frontend.userId = uacc.getUserId();// get the user id
                        frontend.redirectPage = "//todo: link to redirecting";
                        frontend.message = "Sign in complete";
                    }
                    else
                    {
                        frontend.message = backward.first.message;
                    }
                    return Json(frontend);
                }
            }

            return Unauthorized();
        }

        private async Task<PairValue<Mo_Result, UserAccount>> signin(Mo_User_SignIn model)
        {
            PairValue<Mo_Result, UserAccount> back = new PairValue<Mo_Result, UserAccount> { first = new Mo_Result() };

            var req = await Server.get_account_secure(model.username, model.password, true);
            UserAccount uacc = req.first;
            if(uacc != null)
            {
                back.first.result = true;
                back.second = uacc;

                uacc.lastLoggedDate = DateTime.Now; // post time

                //todo: сохранить только определенные значения
                if(uacc.useCount++ == ulong.MaxValue - 1)
                {
                    uacc.useCount = 1;
                    Console.WriteLine("Use count is retrived for id: " + uacc.id);
                }
                                                           //refresh UACC
                await Server.updateProperties(uacc, "lastLoggedDate", "useCount");
            }
            else
            {
                back.first.result = false;
                back.first.message = req.second;
            }
            return back;
        }
    }
}