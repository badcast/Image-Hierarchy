#define HaveMessageToHacker

using System;
using System.Collections.Generic;
using System.Net;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class ConstEnums
{
    public const string Message_To_Hacker = "Go outside my code";
    public static readonly string[] kindSizes = { "Bytes", "KB", "MB", "GB", "TB"};
}

namespace ImageHierarchy.Models
{


    enum UserMode
    {
        Users,
        Moderator,
        Admin
    }
    public class UserAccount
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }

        public string username { get; set; }
        public string password { get; set; }
        public string displayName { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }

        public bool trustedImageContent { get; set; }

        public DateTime registeredDate { get; set; }
        public DateTime lastLoggedDate { get; set; }
        public string role { get; set; }

        public string[] images { get; set; }

        public ulong useCount { get; set; }

        public ulong getUserId()
        {
            ulong mag = 1;
            for(int i = 0; i < id.Length; ++i)
                mag *= id[i];
            return mag;
        }
    }

    public class Mo_User_Content
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string userId { get; set; }
        public string imageId { get; set; }
#if HaveMessageToHacker
        public string messageToHacker { get { return ConstEnums.Message_To_Hacker; } set { } }
#endif
    }

    public class Mo_User_ImageList
    {
        public int length { get; set; }
        public string[] images { get; set; }
#if HaveMessageToHacker
        public string messageToHacker { get { return ConstEnums.Message_To_Hacker; } set { } }
#endif
    }

    public class Mo_User_SignUp
    {
        public string login { get; set; }
        public string password { get; set; }
        public string displayName { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
    }

    public class Mo_User_SignIn
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class Mo_User_Info
    {
        public bool result { get; set; }
        public string message { get; set; }

        public ulong userId { get; set; }
        public string displayName { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public DateTime registeredDate { get; set; }
        public DateTime lastLoggedDate { get; set; }
        public string role { get; set; }
        public ulong useCount { get; set; }
#if HaveMessageToHacker
        public string messageToHacker { get { return ConstEnums.Message_To_Hacker; } set { } }
#endif
    }

    public class Mo_Result
    {
        public bool result { get; set; }
        public object content { get; set; }
        public string message { get; set; }
#if HaveMessageToHacker
        public string messageToHacker { get { return ConstEnums.Message_To_Hacker; } set { } }
#endif
    }

    public class Mo_AuthResult
    {
        public bool result { get; set; }
        public string message { get; set; }
        public string access_token { get; set; }
        public string role { get; set; }
        public ulong userId { get; set; }
        public string redirectPage { get; set; }
        public DateTime loggedDate { get; set; }
        public string refresh_token { get; set; }

#if HaveMessageToHacker
        public string messageToHacker { get { return ConstEnums.Message_To_Hacker; } set { } }
#endif
    }

    public class Mo_TokenValidateResult
    {
        public bool isValid { get; set; }
#if HaveMessageToHacker
        public string messageToHacker { get { return ConstEnums.Message_To_Hacker; } set { } }
#endif
    }
}
