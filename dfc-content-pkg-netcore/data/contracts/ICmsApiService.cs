using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.Content.Pkg.Netcore.Data.Contracts
{
    public interface ICmsApiService
    {
        Task<IList<TApiModel>?> GetSummaryAsync<TApiModel>(string contentType)
            where TApiModel : class, IApiDataModel;

        Task<IList<TApiModel>?> GetSummaryAsync<TApiModel>()
         where TApiModel : class, IApiDataModel;

        Task<TModel?> GetItemAsync<TModel>(string contentType, Guid id)
           where TModel : class, IApiDataModel;

        Task<TModel?> GetItemAsync<TModel>(Uri url)
           where TModel : class, IApiDataModel;

        Task<TModel?> GetItemAsync<TModel, TChild>(Uri url)
            where TModel : class, IBaseContentItemModel<TChild>
            where TChild : class, IBaseContentItemModel<TChild>;

        Task<TChild?> GetContentItemAsync<TChild>(Uri? uri)
            where TChild : class, IBaseContentItemModel<TChild>;

        Task<List<TApiModel>?> GetContentAsync<TApiModel>()
            where TApiModel : class;
    }
}
