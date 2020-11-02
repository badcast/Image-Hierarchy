using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

using MongoDB.Bson;
using MongoDB.Driver;

using ImageHierarchy.Models;

namespace ImageHierarchy
{
    partial class AccountController
    {
        public const string MAIL_HOST = "mail.ru";
        public const string MAIL_LOGIN_CRIDENTIAL = "noreply.20@mail.ru";
        public const string MAIL_PASSWORD_CRIDENTIAL = "^+c9M+sP6)wCs&(";
        public const string MAIL_DISPLAY_NAME = "Truster";

        public static SmtpClient client = new SmtpClient(MAIL_HOST) { Credentials = new NetworkCredential(MAIL_LOGIN_CRIDENTIAL, MAIL_PASSWORD_CRIDENTIAL) };

        public static async Task<string> sendCodeVerify(MailAddress address)
        {
            int code = new Random().Next(100000, 999999);

            MailMessage msg = new MailMessage(new MailAddress(MAIL_LOGIN_CRIDENTIAL), address);
            client.Send(msg);
            return code.ToString();
        }

        [Authorize]
        [Route("/api/mail_checkcode")]
        public async Task<IActionResult> post_checkOut_code(string code)
        {
            return BadRequest();
        }

    }
}
