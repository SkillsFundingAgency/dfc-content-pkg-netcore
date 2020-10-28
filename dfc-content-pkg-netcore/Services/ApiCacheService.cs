using DFC.Content.Pkg.Netcore.Data.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DFC.Content.Pkg.Netcore.Services
{
    public class ApiCacheService : IApiCacheService
    {
        public int Count => CachedItems != null ? CachedItems.Count : 0;

        private Dictionary<Uri, string>? CachedItems { get; set; }

        public void AddOrUpdate(Uri id, object obj)
        {
            if (CachedItems != null && CachedItems.ContainsKey(id))
            {
                CachedItems[id] = JsonConvert.SerializeObject(obj);
                return;
            }

            CachedItems?.Add(id, JsonConvert.SerializeObject(obj));
        }

        public void Clear()
        {
            CachedItems?.Clear();
        }

        public void StartCache()
        {
            CachedItems = new Dictionary<Uri, string>();
        }

        public void StopCache()
        {
            CachedItems = null;
        }

        public void Remove(Uri id)
        {
            CachedItems?.Remove(id);
        }

        public TModel? Retrieve<TModel>(Uri id)
            where TModel : class
        {
            if (CachedItems != null && CachedItems.ContainsKey(id))
            {
                return JsonConvert.DeserializeObject<TModel>(CachedItems[id]);
            }

            return null;
        }

        public TModel? Retrieve<TModel>(Type type, Uri id)
            where TModel : class
        {
            if (CachedItems != null && CachedItems.ContainsKey(id))
            {
                return (TModel?)JsonConvert.DeserializeObject(CachedItems[id], type);
            }

            return null;
        }
    }
}
