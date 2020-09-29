using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DFC.Content.Pkg.Netcore.CmsApiProcessorService.UnitTests.Models
{
    public class ApiItemNoChildrenModel : IBaseContentItemModel
    {
        [JsonProperty("id")]
        public Guid? ItemId { get; set; }

        [JsonIgnore]
        [JsonProperty("pagelocation_UrlName")]
        public string? CanonicalName { get; set; }

        [JsonProperty("skos__prefLabel")]
        public string? Title { get; set; }

        public string? Description { get; set; }

        [JsonProperty(PropertyName = "uri")]
        public Uri? Url { get; set; }
        public ContentLinksModel? ContentLinks { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IList<IBaseContentItemModel> ContentItems { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
