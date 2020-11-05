using DFC.Content.Pkg.Netcore.Data.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DFC.Content.Pkg.Netcore.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class ContentLinksModel
    {
        private readonly JObject? jLinks;

        public ContentLinksModel(JObject? jLinks)
        {
            this.jLinks = jLinks;
        }

        public List<KeyValuePair<string, List<ILinkDetails>>> ContentLinks
        {
            get => LinksPrivate ??= GetLinksFromJObject();

            set => LinksPrivate = value;
        }

        public bool ExcludePageLocation { get; set; }

        private List<KeyValuePair<string, List<ILinkDetails>>>? LinksPrivate { get; set; }

        private static CuriesDetails? GetContentCuriesDetails(JObject links)
        {
            var curies = links["curies"]?.ToString();

            if (string.IsNullOrEmpty(curies))
            {
                return null;
            }

            var curiesList = JsonConvert.DeserializeObject<List<CuriesDetails>>(curies);

            return curiesList.FirstOrDefault();
        }

        private static KeyValuePair<string, List<ILinkDetails>> GetLinkDetailsFromArray(JToken array, string relationshipKey, string? baseHref)
        {
            var links = JsonConvert.DeserializeObject<List<ILinkDetails>>(array.ToString());

            foreach (var link in links)
            {
                link.Uri = new Uri($"{baseHref}{link.Href}");
            }

            return new KeyValuePair<string, List<ILinkDetails>>(relationshipKey, links);
        }

        private List<KeyValuePair<string, List<ILinkDetails>>> GetLinksFromJObject()
        {
            var contLink = new List<KeyValuePair<string, List<ILinkDetails>>>();

            if (jLinks == null)
            {
                return contLink;
            }

            var contentCuriesDetails = GetContentCuriesDetails(jLinks);

            if (contentCuriesDetails == null)
            {
                return contLink;
            }

            foreach (var (key, jValue) in jLinks)
            {
                var value = jValue;

                if (value == null || !key.StartsWith(contentCuriesDetails.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }

                var relationShipKey = key.Replace(
                    $"{contentCuriesDetails.Name}:",
                    string.Empty,
                    StringComparison.CurrentCultureIgnoreCase);

                if (relationShipKey.Equals("HasPageLocation", StringComparison.CurrentCultureIgnoreCase) && ExcludePageLocation)
                {
                    continue;
                }

                if (jValue is JArray)
                {
                    contLink.Add(GetLinkDetailsFromArray(jValue, relationShipKey, contentCuriesDetails.Href));
                }
                else
                {
                    var child = JsonConvert.DeserializeObject<ILinkDetails>(value.ToString());
                    child.Uri = new Uri($"{contentCuriesDetails.Href}{child.Href}");

                    contLink.Add(new KeyValuePair<string, List<ILinkDetails>>(
                        relationShipKey,
                        new List<ILinkDetails>
                        {
                            child,
                        }));
                }
            }

            return contLink;
        }
    }
}
