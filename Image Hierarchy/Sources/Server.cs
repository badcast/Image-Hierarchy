using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;

using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.IO;

using System.Text.Json;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using ImageHierarchy.Models;

namespace ImageHierarchy
{
    public class PairValue<First, Second>
    {
        public First first;
        public Second second;
    }


    public static class Server
    {
        public const int MaxStringField = 30;
        public const string _ADMIN_LOGIN = "admin";
        public const string _ADMIN_PASS = "admin2020";

        public const string serviceDBName = "ImageHDDB";
        public static IMongoCollection<UserAccount> accounts;
        public static MongoClient client;
        public static IMongoDatabase database;
        public static GridFSBucket gridFS;// Storage
        static Server()
        {
            string connectionString = "mongodb://localhost:27017/" + serviceDBName;
            var connection = new MongoUrlBuilder(connectionString);

            __REFRESH:
            try
            {
            }
            catch
            {
                goto __REFRESH;

            }

            client = new MongoClient(connectionString);

            database = client.GetDatabase(connection.DatabaseName);

            gridFS = new GridFSBucket(database);

            accounts = database.GetCollection<UserAccount>("Accounts");

            //Устанавливаем в базу администратора 
            UserAccount uacc = get_account_secure(_ADMIN_LOGIN, _ADMIN_PASS, true).GetAwaiter().GetResult().first;
            if(uacc == null)
            {
                uacc = register_account(new Mo_User_SignUp()
                {
                    displayName = "Администратор",
                    email = "admin@localhost",
                    login = _ADMIN_LOGIN,
                    password = "admin2020",
                    phoneNumber = "+77021094814", // ввод корректен в формате: +7() или 8
                }).GetAwaiter().GetResult();

                if(uacc == null)
                {
                    delete_account(_ADMIN_LOGIN).GetAwaiter().GetResult();
                }
                else
                {
                    uacc.trustedImageContent = true;
                    uacc.role = "admin";
                    update(uacc).GetAwaiter().GetResult();
                }
            }

            if(uacc == null)
            {
                //todo: Администратор не установлен в системе (сервер)! 
                Console.WriteLine("Администратор отсутствует в системе (сервер)");
            }
            else
            {
                //Приветствие администратора
                Console.WriteLine(string.Format("\nДобро пожаловать Админ!\n\n\tЛогин (для входа): {0}\n\tПароль (для входа): {1}\n\n",
                                    uacc.username, uacc.password));
            }
        }

        public static bool validLatinChars(string val)
        {
            if(string.IsNullOrEmpty(val))
                return false;
            int i = val.Length - 1;
            for(; i >= 0; --i)
                if(!(char.ToLower(val[i]) >= 'a' && char.ToLower(val[i]) <= 'z'))
                    return false;
            return true;
        }

        public static bool validLogin(string val)
        {
            if(string.IsNullOrWhiteSpace(val) || val.Length > MaxStringField)
                return false;
            int i = val.Length - 1;

            if(char.IsNumber(val[i]))
                return false;

            for(; i >= 0; --i)
                if(!(char.ToLower(val[i]) >= 'a' && char.ToLower(val[i]) <= 'z'))
                    return false;
            return true;
        }
        public static bool validPassword(string val)
        {
            if(string.IsNullOrWhiteSpace(val) || val.Length <= 6 || val.Length > MaxStringField)
                return false;
            int i = val.Length - 1;
            for(; i >= 0; --i)
                if(!(char.ToLower(val[i]) != '\0' && !char.IsWhiteSpace(val[i]) && !char.IsSeparator(val[i])))
                    return false;
            return true;
        }
        public static bool validDisplayName(string val)
        {
            return val.Length > 4 && val.Length <= MaxStringField && !val.Contains(' ') &&
                                                                     !val.Contains('\t') &&
                                                                     !val.Contains('\n');
        }
        public static bool validPhoneNumber(string val)
        {
            if(string.IsNullOrWhiteSpace(val) || val.Length <= 10 || val.Length > MaxStringField)
                return false;
            int i = 0;
            if(val[i] == '+')
                ++i;
            for(; i < val.Length; ++i)
                if(!char.IsNumber(val[i]))
                    return false;
            return true;
        }
        public static bool validMail(string val)
        {
            return validMail(val, out MailAddress ml);
        }
        public static bool validMail(string val, out MailAddress mail)
        {
            mail = null;
            try
            {
                mail = new MailAddress(val);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> delete_account(string loginOrEmail)
        {
            UserAccount uacc;
            if((uacc = await get_account(loginOrEmail)) == null)
            {
                //todo: логгировать
                return false;
            }

            return (await delete(uacc.id)).DeletedCount > 0;
        }

        public static async Task<UserAccount> register_account(Mo_User_SignUp field)
        {
            if(await get_account(field.login) != null)
            {
                //todo: логгировать
                return null;
            }

            UserAccount account = new UserAccount();
            account.username = field.login.ToLower();
            account.password = field.password;
            account.displayName = field.displayName;
            account.phoneNumber = field.phoneNumber;
            account.email = field.email;
            account.registeredDate = DateTime.Now;
            account.images = new string[0];
            await accounts.InsertOneAsync(account);

            return account;
        }

        public static async Task<UserAccount> get_account(string loginOrEmail)
        {
            PairValue<UserAccount, string> result = await get_account_secure(loginOrEmail, null, false);
            return result.first;
        }
        public static async Task<PairValue<UserAccount, string>> get_account_secure(string usernameOrMail, string password, bool checkPassword)
        {
            var builder = Builders<UserAccount>.Filter;
            var filter = builder.Empty;
            var result = new PairValue<UserAccount, string>();

            usernameOrMail = usernameOrMail?.ToLower();
            if((validLogin(usernameOrMail)))
            {
                filter = builder.Eq("username", usernameOrMail);
            }
            else if(validMail(usernameOrMail, out MailAddress mail))
            {
                filter = builder.Eq("mail", usernameOrMail);
            }
            else
            {
                result.second = "Invalid login";
                return result;
            }

            if(checkPassword && !validPassword(password))
            {
                result.second = "Invalid password";
                return result;
            }

            result.first = await accounts.Find(filter).FirstOrDefaultAsync();
            if(result.first != null)
            {
                if(checkPassword && result.first.password != password)
                {
                    result.second = "Incorrect password";
                    result.first = null;
                    return result;
                }

                result.first.role = result.first.role ?? "User"; // replace a null value
            }
            else
            {
                result.second = "Account is not found";
            }

            return result;
        }

        public static async Task<UserAccount> get_account_id(string id)
        {
            return await accounts.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();
        }

        public static async Task<UserAccount> GetProductFromName(string name)
        {
            return await accounts.Find(new BsonDocument("name", new BsonString(name))).FirstOrDefaultAsync();
        }

        // обновление документа
        public static async Task<ReplaceOneResult> update(UserAccount uacc)
        {
            return await accounts.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(uacc.id)), uacc);
        }

        public static async Task<ReplaceOneResult> updateProperties(UserAccount uacc, params string[] propertyNames)
        {
            System.Reflection.PropertyInfo[] fields;
            Type type;
            int len;
            var builder = Builders<UserAccount>.Filter;
            FilterDefinition<UserAccount> filter = builder.Empty;
            type = uacc.GetType();
            fields = new System.Reflection.PropertyInfo[propertyNames.Length];
            for(len = fields.Length - 1; len > ~0; --len)
            {
                fields[len] = type.GetProperty(propertyNames[len]);
                filter |= builder.Eq(propertyNames[len], BsonValue.Create(fields[len].GetValue(uacc)));
            }
            filter = builder.Eq("_id", new ObjectId(uacc.id)) & filter;
            var rr = await accounts.ReplaceOneAsync(filter, uacc);
            return rr;
        }

        // удаление документа
        public static async Task<DeleteResult> delete(string id)
        {
            return await accounts.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }

        public static void register_route(IEndpointRouteBuilder route)
        {
            //todo: remove line
        }
    }
}
