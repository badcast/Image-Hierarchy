using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;

using MongoDB.Driver;
using MongoDB.Bson;

using ImageHierarchy.Models;

namespace ImageHierarchy
{
    /*
    public static partial class Service
    {
        private static void token_get_date(string token, out DateTime dateTime)
        {
            byte[] data = Convert.FromBase64String(token);
            dateTime = new DateTime(BitConverter.ToInt64(data));
        }
        private bool register_token(string accountId, string token)
        {
            if(!active_tokens.ContainsKey(token))
            {
                UserAccount acc = get_account(accountId).GetAwaiter().GetResult();
                DateTime t;
                token_get_date(token, out t);
                acc.lastLoggedDate = t;
                active_tokens.Add(token, accountId);
                //update
                update(acc).GetAwaiter().GetResult();
                return true;
            }
            return false;
        }

        public string create_free_tocken(DateTime date)
        {
            //randomizeTocken
            byte[] tokens = new byte[32];
            string str;
            RandomNumberGenerator r = RandomNumberGenerator.Create();
            byte[] tt = BitConverter.GetBytes(date.Ticks);
            for(int i = 0; i < 8; ++i)
                tokens[i] = tt[i];
            do
            {
                r.GetBytes(tokens, 8, tokens.Length-8);
                str = Convert.ToBase64String(tokens);
            } while(active_tokens.ContainsKey(str));

            return str;
        }

        public string get_token(string accountId)
        {
            string token = null;
            foreach(var tt in active_tokens)
            {
                if(tt.Value == accountId)
                {
                    token = tt.Key;
                    break;
                }
            }

            if(token == null)
            {
                token = create_free_tocken(DateTime.Now);
                if(!register_token(accountId, token))
                    token = null;
            }
            return token;
        }

        public string get_accountId_from_token(string token)
        {
            string acc;

            if(active_tokens.TryGetValue(token, out acc))
            {
                //todo: go event
            }

            return acc;
        }

        public bool exist_token(string token)
        {
            return get_accountId_from_token(token) != null;
        }

        public bool break_token(string token)
        {
            string accId;
            bool result;

            if(result = active_tokens.TryGetValue(token, out accId))
            {
                result = active_tokens.Remove(token); // remove 
                update(get_account(accId).GetAwaiter().GetResult()).GetAwaiter();
            }

            return result;
        }
    }*/
}
