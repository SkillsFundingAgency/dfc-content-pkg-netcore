using DFC.Content.Pkg.Netcore.Data.Models;
using System;
using System.Collections.Generic;

namespace DFC.Content.Pkg.Netcore.Data.Contracts
{
    public interface IBaseContentItemModel : IApiDataModel
    {
        ContentLinksModel? ContentLinks { get; set; }

        IList<IBaseContentItemModel> ContentItems { get; set; }

        Guid? ItemId { get; set; }

        string? ContentType { get; set; }
    }
}
