﻿using DFC.Content.Pkg.Netcore.Data.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.Content.Pkg.Netcore.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class BaseContentItemModel : IBaseContentItemModel
    {
        [JsonProperty("Uri")]
        public Uri? Url { get; set; }

        [JsonProperty("id")]
        public Guid? ItemId { get; set; }

        [JsonProperty("skos__prefLabel")]
        public string? Title { get; set; }

        [JsonProperty(PropertyName = "ModifiedDate")]
        public DateTime Published { get; set; }

        public DateTime? CreatedDate { get; set; }

        [JsonProperty("_links")]
        public JObject? Links { get; set; }

        [JsonIgnore]
        public ContentLinksModel? ContentLinks
        {
            get => PrivateLinksModel ??= new ContentLinksModel(Links);

            set => PrivateLinksModel = value;
        }

        public IList<IBaseContentItemModel> ContentItems { get; set; } = new List<IBaseContentItemModel>();

        public string? ContentType { get; set; }

        [JsonIgnore]
        private ContentLinksModel? PrivateLinksModel { get; set; }
    }
}
