using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Text.Json;
using System.IO;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;

using MongoDB.Bson;
using MongoDB.Driver.GridFS;

using ImageHierarchy.Models;

namespace ImageHierarchy
{
    partial class AccountController
    {
        async Task<IActionResult> extern_upload()
        {
            //UserAccount uacc = await Service.get_account(User.FindFirst("login").Value);
            if(!Request.ContentLength.HasValue || Request.ContentLength > UploadMaxSize || Request.ContentLength == 0)
            {
                return StatusCode(411);
            }

            string uploadedFileName = null;
            if(Request.Headers.TryGetValue("name", out Microsoft.Extensions.Primitives.StringValues val))
                if(val.Count != 0)
                    uploadedFileName = Uri.UnescapeDataString(val.ToString());

            if(string.IsNullOrEmpty(uploadedFileName))
            {
                //generate new name
                lock(_lock)
                {
                    DateTime t = DateTime.UtcNow;
                    uploadedFileName = "upd_date:" + t.ToString();
                    uploadedFileName += ":ms" + t.Millisecond;
                    uploadedFileName = uploadedFileName.Replace(' ', '.');
                    uploadedFileName += ".image";
                }

            }

            //todo: Upload images
            Stream reqStream = Request.Body;
            MemoryStream memory = new MemoryStream((int)(Request.ContentLength));
            byte[] download = new byte[1024];
            int temp;
            do
            {
                temp = await reqStream.ReadAsync(download, 0, download.Length);
                for(int i = 0; i < temp; ++i)
                    memory.Write(download, 0, temp);
                System.Threading.Thread.Sleep(1);
            } while(temp > 0);

            //Конвертирование в MB
            string hReadable = ((memory.Length / 1000000.0f).ToString());
            temp = hReadable.IndexOf(',');
            if(temp > 0 && hReadable.Length - (temp + 2) > 0)
            {
                hReadable = hReadable.Remove(temp + 2, hReadable.Length - (temp + 2));
            }
            hReadable += "MB";

            UserAccount uacc = User.Identity.IsAuthenticated ? await Server.get_account(User.FindFirst("username").Value) : null;
            ObjectId __id = await Server.gridFS.UploadFromStreamAsync(uploadedFileName, memory);
            
            return Json(new Mo_Result()
            {
                message = "Image uploaded!",
                result = true,
                content = new
                {
                    uploaded = memory.Length,
                    humanReadable = hReadable,
                    filename = uploadedFileName,
                    imageId = __id.ToString(),
                    devMode = true, // это значит ID изображения экспериментальная
                    hasUser = uacc != null,
                    userId = uacc != null ? uacc.getUserId() : 0
                }
            }); 
        }
    }
}
