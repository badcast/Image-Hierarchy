using ImageHierarchy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ImageHierarchy
{
    public partial class AccountController : Controller
    {
        static object _lock = new object();
        const int UploadMaxSize = 5000000; // 5 Megabytes

        [Route("api/signin")]
        public async Task<IActionResult> POST_SignIn()
        {
            try
            {
                return await extern_signin();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Unauthorized();
        }

        [Route("api/signup")]
        public async Task<IActionResult> POST_SignUp()
        {
            return await extern_signup();
        }

        [Route("api/Upload")]
        public async Task<IActionResult> POST_upload_image()
        {
            return await extern_upload();
        }

        [Route("api/valid")]
        public IActionResult GET_Valid()
        {
            bool isValid = false;
            string type = "unknown";
            if(Request.Query.ContainsKey("access_token"))
            {
                string access_token = Request.Query["access_token"].ToString();
                isValid = JWT_Token_Controller.check_token(access_token);
                type = "Token Validator";
            }

            return Json(new {
                valid = isValid,
                validator = type,
                messageToHacker = ConstEnums.Message_To_Hacker
            });
        }

        [Route("api/GetUserInfo")]
        [Authorize()]
        public async Task<IActionResult> GET_user_info()
        {
            return await extern_getinfo();
        }

        [Authorize()]
        [Route("api/GetDisplayName")]
        public async Task<IActionResult> GET_displayName()
        {
            UserAccount uacc = await Server.get_account(User.FindFirst("username").Value);
            return Json(new Mo_Result() { message = "ok", result = true, content = uacc.displayName });
        }

        [Authorize()]
        [Route("api/GetPhoneNumber")]
        public async Task<IActionResult> GET_phoneNumber()
        {
            UserAccount uacc = await Server.get_account(User.FindFirst("username").Value);
            return Json(new Mo_Result() { message = "ok", result = true, content = uacc.phoneNumber });
        }

        [Authorize()]
        [Route("api/GetEmail")]
        public async Task<IActionResult> GET_email()
        {
            UserAccount uacc = await Server.get_account(User.FindFirst("username").Value);
            return Json(new Mo_Result() { message = "ok", result = true, content = uacc.email });
        }

        [Authorize()]
        [Route("api/GetRegisteredDate")]
        public async Task<IActionResult> GET_registeredDate()
        {
            UserAccount uacc = await Server.get_account(User.FindFirst("username").Value);
            return Json(new Mo_Result() { message = "ok", result = true, content = uacc.registeredDate.ToUniversalTime().ToLongTimeString() });
        }

        [Authorize()]
        [Route("api/GetLastLoggedDate")]
        public async Task<IActionResult> GET_lastLoggedDate()
        {
            UserAccount uacc = await Server.get_account(User.FindFirst("username").Value);
            return Json(new Mo_Result() { message = "ok", result = true, content = uacc.lastLoggedDate.ToUniversalTime().ToLongTimeString() });
        }

        [Authorize]
        [Route("api/GetVerifyUser")]
        public async Task<IActionResult> GET_verifyUser()
        {
            UserAccount uacc = await Server.get_account(User.FindFirst("username").Value);
            return Json(new
            {
                userId = uacc.getUserId(),
                verify = uacc.trustedImageContent
            });
        }

        [Authorize]
        [Route("api/GetUserImages")]
        public async Task<IActionResult> GET_images()
        {
            UserAccount uacc = await Server.get_account(User.FindFirst("username").Value);

            if(uacc.images == null)
            {
                uacc.images = new string[0];
                await Server.updateProperties(uacc, "images");
            }

            return Json(new Mo_User_ImageList()
            {
                length = uacc.images.Length,
                images = uacc.images
            });
        }
    }
}
