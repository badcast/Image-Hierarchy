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
        async Task<IActionResult> extern_getinfo()
        {
            UserAccount uacc = await Server.get_account(User.FindFirst("login").Value);
            Mo_User_Info usifo = new Mo_User_Info() { result = false };
            if(uacc == null)
                return Unauthorized();
            else
            {
                usifo.message = "ok";
                usifo.lastLoggedDate = uacc.lastLoggedDate;
                usifo.registeredDate = uacc.registeredDate;
                usifo.phoneNumber = uacc.phoneNumber;
                usifo.email = uacc.email;
                usifo.displayName = uacc.displayName;
                usifo.role = uacc.role;
                usifo.userId = uacc.getUserId();
                usifo.useCount = uacc.useCount;

                usifo.result = true;
            }


            return Json(usifo);
        }
    }
}