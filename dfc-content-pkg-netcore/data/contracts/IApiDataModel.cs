using Newtonsoft.Json;
using System;

namespace dfc_content_pkg_netcore.contracts
{
    public interface IApiDataModel
    {
        [JsonProperty("Uri")]
        Uri? Url { get; set; }
    }
}
