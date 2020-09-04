using dfc_content_pkg_netcore.models;
using System.Collections.Generic;

namespace dfc_content_pkg_netcore.contracts
{
    public interface ICmsApiDataModel
    {
        ContentLinksModel? ContentLinks { get; set; }

        IList<BaseContentItemModel> ContentItems { get; set; }
    }
}
