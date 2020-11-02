using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Text.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;

using MongoDB.Bson;

using ImageHierarchy.Models;

namespace ImageHierarchy
{
    partial class AccountController
    {
        async Task<IActionResult> extern_signup()
        { 
            Mo_User_SignUp backend; 
            Mo_Result frontend;
            if((Request.ContentLength.HasValue && Request.ContentLength > 0 || Request.Body.CanRead))
            {
                string reqJson;
                using(var reader = new System.IO.StreamReader(Request.Body))
                    reqJson = await reader.ReadToEndAsync();
                if(!string.IsNullOrEmpty(reqJson))
                {
                    backend = JsonSerializer.Deserialize<Mo_User_SignUp>(reqJson);
                    frontend = await signup(backend);
                    if(frontend.result)
                    {
                        //complete
                    }
                    return Json(frontend);
                }
            }
            return BadRequest();
        }

        public async Task<Mo_Result> signup(Mo_User_SignUp model)
        {
            Mo_Result response = new Mo_Result();
            UserAccount uacc = null;
            if(Server.validLogin(model.login))
            {
                uacc = Server.get_account(model.login).GetAwaiter().GetResult();
                if(uacc == null)
                {
                    uacc = Server.get_account(model.email).GetAwaiter().GetResult();
                    if(uacc != null)
                    {
                        response.message = "Email is registered";
                        goto break_point;
                    }
                }
            }
            else
            {
                response.message = "Invalid login";
                goto break_point;
            }
            //is not an registered
            if(uacc == null)
            {
                if(Server.validPassword(model.password))
                {
                    if(!Server.validDisplayName(model.displayName))
                    {
                        response.message = "Invalid Display Name";
                        goto break_point;
                    }

                    if(!Server.validPhoneNumber(model.phoneNumber))
                    {
                        response.message = "Invalid Phone number";
                        goto break_point;
                    }
                    if(!Server.validMail(model.email, out MailAddress ml))
                    {
                        response.message = "Invalid Mail format";
                        goto break_point;
                    }

                    //register account
                    UserAccount registeredAccount = await Server.register_account(model);
                    response.result = true;
                    response.message = "Account registered";
                }
                else
                {
                    response.message = "Invalid password";
                    goto break_point;
                }
            }
            else // have a registered
            {
                response.message = "Have a account";
            }
            break_point:
            return response;
        }
    }
}
