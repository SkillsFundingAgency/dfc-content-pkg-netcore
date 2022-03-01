using System;

namespace DFC.Content.Pkg.Netcore.Data.Contracts
{
    public interface IApiCacheService
    {
        int Count { get; }

        void AddOrUpdate(string key, object obj);

        void Clear();

        void StartCache();

        void StopCache();

        void Remove(string key);

        TModel? Retrieve<TModel>(string key)
            where TModel : class;

        TModel? Retrieve<TModel>(Type type, string key)
            where TModel : class;
    }
}
