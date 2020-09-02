using dfc_content_pkg_netcore.models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dfc_content_pkg_netcore.contracts
{
    public interface ICmsApiService
    {
        Task<IList<T>> GetSummaryAsync<T>()
            where T : class;

        Task<T> GetItemAsync<T>(Uri url)
            where T : class, IPagesApiDataModel;

        Task<ApiContentItemModel> GetContentItemAsync(LinkDetails details);

        Task<ApiContentItemModel> GetContentItemAsync(Uri uri);

        Task<List<T>> GetContentAsync<T>()
            where T : class;
    }
}
