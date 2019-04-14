using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Infrastructure.Extensions
{
    public static class SessionExtensions
    {
        public static T SetJson<T>(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
            return session.GetJson<T>(key);
        }

        public static T GetJson<T>(this ISession session, string key)
        {
            var sessionData = session.GetString(key);
            return sessionData == null? default(T) : JsonConvert.DeserializeObject<T>(sessionData);
        }
    }
}
