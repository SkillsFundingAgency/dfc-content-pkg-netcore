using dfc_content_pkg_netcore.models;
using System.Collections.Generic;

namespace dfc_content_pkg_netcore.contracts
{
    public interface IPagesApiDataModel
    {
        ContentLinksModel? ContentLinks { get; set; }

        IList<ApiContentItemModel> ContentItems { get; set; }
    }
}
