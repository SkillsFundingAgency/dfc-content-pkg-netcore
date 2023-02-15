using DFC.Content.Pkg.Netcore.Data.Models;
using System.Collections.Generic;

namespace DFC.Content.Pkg.Netcore.Data.Contracts
{
    public interface ICmsApiDataModel
    {
        ContentLinksModel? ContentLinks { get; set; }

        IList<BaseContentItemModel> ContentItems { get; set; }
    }
}
