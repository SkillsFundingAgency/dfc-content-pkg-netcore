using DFC.Content.Pkg.Netcore.Data.Contracts;
using Newtonsoft.Json;
using System;

namespace DFC.Content.Pkg.Netcore.CmsApiProcessorService.UnitTests.Models
{
    public class ApiSummaryModel : IApiDataModel
    {
        [JsonProperty(PropertyName = "uri")]
        public Uri? Url { get; set; }

        [JsonProperty(PropertyName = "skos__prefLabel")]
        public string? Title { get; set; }

        public DateTime? CreatedDate { get; set; }

        [JsonProperty(PropertyName = "ModifiedDate")]
        public DateTime Published { get; set; }
    }
}
