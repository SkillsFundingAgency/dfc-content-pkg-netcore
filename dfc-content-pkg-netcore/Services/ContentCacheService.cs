using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.enums;
using Microsoft.Extensions.Logging;
using NHibernate.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DFC.Content.Pkg.Netcore.Services
{
    public class ContentCacheService : IContentCacheService
    {
        private readonly ILogger<ContentCacheService> logger;

        public ContentCacheService(ILogger<ContentCacheService> logger)
        {
            this.logger = logger;
        }

        private IDictionary<Guid, List<Guid>> ContentItems { get; set; } = new Dictionary<Guid, List<Guid>>();

        public ContentCacheStatus CheckIsContentItem(Guid contentItemId)
        {
            if (ContentItems.Any(z => z.Key == contentItemId))
            {
                return ContentCacheStatus.Content;
            }

            foreach (var contentId in ContentItems.Keys)
            {
                if (ContentItems[contentId].Contains(contentItemId))
                {
                    return ContentCacheStatus.ContentItem;
                }
            }

            return ContentCacheStatus.NotFound;
        }

        public void Clear()
        {
            logger.LogInformation($"Clear content cache called.");
            ContentItems.Clear();
        }

        public IList<Guid> GetContentIdsContainingContentItemId(Guid contentItemId)
        {
            var contentIds = new List<Guid>();

            foreach (var contentId in ContentItems.Keys)
            {
                if (ContentItems[contentId].Contains(contentItemId))
                {
                    contentIds.Add(contentId);
                }
            }

            return contentIds;
        }

        public void Remove(Guid contentId)
        {
            logger.LogInformation($"Removing Content {contentId} from cache");

            if (ContentItems.ContainsKey(contentId))
            {
                ContentItems.Remove(contentId);
            }
        }

        public void RemoveContentItem(Guid contentId, Guid contentItemId)
        {
            if (ContentItems.ContainsKey(contentId))
            {
                ContentItems[contentId].Remove(contentItemId);
            }
        }

        public void AddOrReplace(Guid contentId, List<Guid> contentItemIds)
        {
            if (ContentItems.ContainsKey(contentId))
            {
                ContentItems[contentId] = contentItemIds;
            }
            else
            {
                ContentItems.Add(contentId, contentItemIds);
            }
        }
    }
}
