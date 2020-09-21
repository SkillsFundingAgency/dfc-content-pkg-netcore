using DFC.Content.Pkg.Netcore.Data.Contracts;
using Newtonsoft.Json;
using System;

namespace DFC.Content.Pkg.Netcore.CmsApiProcessorService.UnitTests.Models
{
    public class ApiItemNoChildrenModel : IApiDataModel
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
    }
}
