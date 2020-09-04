using dfc_content_pkg_netcore.contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace dfc_content_pkg_netcore.models
{
    public class BaseContentItemModel : IApiDataModel
    {
        public Uri Url { get; set; }

        [JsonProperty("id")]
        public Guid? ItemId { get; set; }

        [JsonProperty("skos__prefLabel")]
        public string Content { get; set; }

        [JsonProperty(PropertyName = "ModifiedDate")]
        public DateTime Published { get; set; }

        public DateTime? CreatedDate { get; set; }

        [JsonProperty("_links")]
        public JObject Links { get; set; }

        [JsonIgnore]
        public ContentLinksModel ContentLinks
        {
            get => PrivateLinksModel ??= new ContentLinksModel(Links);

            set => PrivateLinksModel = value;
        }

        public IList<BaseContentItemModel> ContentItems { get; set; } = new List<BaseContentItemModel>();

        [JsonIgnore]
        private ContentLinksModel PrivateLinksModel { get; set; }
    }
}
