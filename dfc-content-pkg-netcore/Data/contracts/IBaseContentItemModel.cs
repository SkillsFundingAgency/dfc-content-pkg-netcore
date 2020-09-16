using dfc_content_pkg_netcore.models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace dfc_content_pkg_netcore.contracts
{
    public interface IBaseContentItemModel<T> : IApiDataModel
        where T: class
    {
        Uri Url { get; set; }

        [JsonProperty("id")]
        Guid? ItemId { get; set; }

        [JsonProperty("skos__prefLabel")]
        string Content { get; set; }

        [JsonProperty("contentType")]
        string ContentType { get; set; }

        [JsonProperty(PropertyName = "ModifiedDate")]
        DateTime Published { get; set; }

        DateTime? CreatedDate { get; set; }

        [JsonProperty("_links")]
        JObject Links { get; set; }

        [JsonIgnore]
        ContentLinksModel ContentLinks
        {
            get => PrivateLinksModel ??= new ContentLinksModel(Links);

            set => PrivateLinksModel = value;
        }

        IList<T> ContentItems { get; set; }

        [JsonIgnore]
        ContentLinksModel PrivateLinksModel { get; set; }
    }
}
