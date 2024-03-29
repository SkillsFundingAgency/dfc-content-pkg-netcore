﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DFC.Content.Pkg.Netcore.Data.Models;

namespace DFC.Content.Pkg.Netcore.Data.Contracts
{
    public interface ICmsApiService
    {
        Task<IList<TApiModel>?> GetSummaryAsync<TApiModel>(string contentType)
            where TApiModel : class, IApiDataModel;

        Task<IList<TApiModel>?> GetSummaryAsync<TApiModel>(string contentType, bool clearCache)
            where TApiModel : class, IApiDataModel;

        Task<IList<TApiModel>?> GetSummaryAsync<TApiModel>()
         where TApiModel : class, IApiDataModel;

        Task<TModel?> GetItemAsync<TModel>(string contentType, Guid id)
           where TModel : class, IApiDataModel;

        Task<TModel?> GetItemAsync<TModel>(Uri url)
           where TModel : class, IBaseContentItemModel;

        Task<TModel?> GetItemAsync<TModel>(Uri url, CmsApiOptions options)
            where TModel : class, IBaseContentItemModel;

        Task<TChild?> GetContentItemAsync<TChild>(Type type, Uri? uri)
             where TChild : class, IBaseContentItemModel;

        Task<TChild?> GetContentItemAsync<TChild>(Uri? uri)
            where TChild : class, IBaseContentItemModel;

        Task<List<TApiModel>?> GetContentAsync<TApiModel>()
            where TApiModel : class;
    }
}
