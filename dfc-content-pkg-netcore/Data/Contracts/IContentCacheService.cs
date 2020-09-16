using DFC.Content.Pkg.Netcore.Data.enums;
using System;
using System.Collections.Generic;

namespace DFC.Content.Pkg.Netcore.Data.Contracts
{
    public interface IContentCacheService
    {
        ContentCacheStatus CheckIsContentItem(Guid contentItemId);

        void Clear();

        IList<Guid> GetContentIdsContainingContentItemId(Guid contentItemId);

        void Remove(Guid contentId);

        void RemoveContentItem(Guid contentId, Guid contentItemId);

        void AddOrReplace(Guid contentId, List<Guid> contentItemIds);
    }
}
