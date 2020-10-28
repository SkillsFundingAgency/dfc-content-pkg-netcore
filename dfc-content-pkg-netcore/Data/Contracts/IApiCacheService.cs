using System;

namespace DFC.Content.Pkg.Netcore.Data.Contracts
{
    public interface IApiCacheService
    {
        int Count { get; }

        void AddOrUpdate(Uri id, object obj);

        void Clear();

        void StartCache();

        void StopCache();

        void Remove(Uri id);

        TModel? Retrieve<TModel>(Uri id)
            where TModel : class;

        TModel? Retrieve<TModel>(Type type, Uri id)
            where TModel : class;
    }
}
