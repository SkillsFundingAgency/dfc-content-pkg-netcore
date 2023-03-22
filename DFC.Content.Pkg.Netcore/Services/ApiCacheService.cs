using DFC.Content.Pkg.Netcore.Data.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DFC.Content.Pkg.Netcore.Services
{
    public class ApiCacheService : IApiCacheService
    {
        public int Count => CachedItems != null ? CachedItems.Count : 0;

        private Dictionary<string, string>? CachedItems { get; set; }

        public void AddOrUpdate(string key, object obj)
        {
            if (CachedItems != null && CachedItems.ContainsKey(key))
            {
                CachedItems[key] = JsonConvert.SerializeObject(obj);
                return;
            }

            CachedItems?.Add(key, JsonConvert.SerializeObject(obj));
        }

        public void Clear()
        {
            CachedItems?.Clear();
        }

        public void StartCache()
        {
            CachedItems = new Dictionary<string, string>();
        }

        public void StopCache()
        {
            CachedItems = null;
        }

        public void Remove(string key)
        {
            CachedItems?.Remove(key);
        }

        public TModel? Retrieve<TModel>(string key)
            where TModel : class
        {
            if (CachedItems != null && CachedItems.ContainsKey(key))
            {
                return JsonConvert.DeserializeObject<TModel>(CachedItems[key]);
            }

            return null;
        }

        public TModel? Retrieve<TModel>(Type type, string key)
            where TModel : class
        {
            if (CachedItems != null && CachedItems.ContainsKey(key))
            {
                return (TModel?)JsonConvert.DeserializeObject(CachedItems[key], type);
            }

            return null;
        }
    }
}
