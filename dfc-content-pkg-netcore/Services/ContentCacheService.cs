using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Enums;
using DFC.Content.Pkg.Netcore.Data.Models;
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

        private IDictionary<KeyValuePair<string, Guid>, List<Guid>> ContentItems { get; set; } = new Dictionary<KeyValuePair<string, Guid>, List<Guid>>();

        public IEnumerable<ContentCacheResult> GetContentCacheStatus(Guid contentItemId)
        {
            var listToReturn = new List<ContentCacheResult>();

            var contentResult = ContentItems.Where(x => x.Key.Value == contentItemId);

            if (contentResult != null && contentResult.Any())
            {
                foreach (var result in contentResult)
                {
                    listToReturn.Add(new ContentCacheResult() { ContentType = result.Key.Key, Result = ContentCacheStatus.Content });
                }
            }

            var contentItemResult = ContentItems.Where(x => x.Value.Contains(contentItemId));

            if (contentItemResult != null && contentItemResult.Any())
            {
                foreach (var result in contentItemResult)
                {
                    listToReturn.Add(new ContentCacheResult() { ParentContentId = result.Key.Value, ContentType = result.Key.Key, Result = ContentCacheStatus.ContentItem });
                }
            }

            if (!listToReturn.Any())
            {
                listToReturn.Add(new ContentCacheResult { ContentType = string.Empty, Result = ContentCacheStatus.NotFound });
            }

            return listToReturn;
        }

        public ContentCacheStatus CheckIsContentItem(Guid contentItemId)
        {
            bool isContent = false;

            if (ContentItems.Any(z => z.Key.Value == contentItemId))
            {
                isContent = true;
            }

            foreach (var contentId in ContentItems.Keys)
            {
                if (ContentItems[contentId].Contains(contentItemId))
                {
                    if (isContent)
                    {
                        return ContentCacheStatus.Both;
                    }

                    return ContentCacheStatus.ContentItem;
                }
            }

            if (isContent)
            {
                return ContentCacheStatus.Content;
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
            var contentItemIdsContaining = ContentItems.Where(x => x.Value.Contains(contentItemId)).Select(z => z.Key.Value);

            return contentItemIdsContaining.ToList();
        }

        public void Remove(Guid contentId)
        {
            logger.LogInformation($"Removing Content {contentId} from cache");

            var matchingKeys = ContentItems.Where(x => x.Key.Value == contentId).Select(z => z.Key);

            foreach (var key in matchingKeys)
            {
                ContentItems.Remove(key);
            }
        }

        public void RemoveContentItem(Guid contentId, Guid contentItemId)
        {
            if (ContentItems.Any(x => x.Key.Value == contentId))
            {
                var matchingKeys = ContentItems.Where(x => x.Key.Value == contentId && x.Value.Contains(contentItemId)).Select(z => z.Key);

                foreach (var key in matchingKeys)
                {
                    ContentItems[key].Remove(contentItemId);
                }
            }
        }

        public void AddOrReplace(Guid contentId, List<Guid> contentItemIds, string parentContentType = "default")
        {
            var hasKey = ContentItems.Any(x => x.Key.Value == contentId);

            if (hasKey)
            {
                ContentItems[ContentItems.FirstOrDefault(x => x.Key.Value == contentId).Key] = contentItemIds;
            }
            else
            {
                ContentItems.Add(new KeyValuePair<string, Guid>(parentContentType, contentId), contentItemIds);
            }
        }
    }
}
