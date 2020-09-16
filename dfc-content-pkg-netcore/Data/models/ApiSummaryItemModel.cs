using dfc_content_pkg_netcore.contracts;
using Newtonsoft.Json;
using System;

namespace dfc_content_pkg_netcore.models
{
    public class ApiSummaryItemModel : IApiDataModel
    {
        [JsonProperty(PropertyName = "uri")]
        public Uri? Url { get; set; }

        [JsonProperty(PropertyName = "skos__prefLabel")]
        public string? Title { get; set; }

        public string CanonicalName
        {
            get => Title;

            set => Title = value;
        }

        public DateTime? CreatedDate { get; set; }

        [JsonProperty(PropertyName = "ModifiedDate")]
        public DateTime Published { get; set; }
    }
}
