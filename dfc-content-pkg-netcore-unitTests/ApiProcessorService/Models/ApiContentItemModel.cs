﻿using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DFC.Content.Pkg.Netcore.ApiProcessorService.UnitTests.Models
{
    public class ApiContentItemModel : IBaseContentItemModel
    {
        [JsonProperty("Uri")]
        public Uri? Url { get; set; }

        [JsonProperty("id")]
        public Guid? ItemId { get; set; }

        [JsonProperty("skos__prefLabel")]
        public string? Title { get; set; }

        public string? BreadcrumbText { get; set; }

        public string? Content { get; set; }

        public int? Justify { get; set; }

        public string? Alignment { get; set; }

        public int? Ordinal { get; set; }

        public int? Size { get; set; }

        [JsonProperty(PropertyName = "ModifiedDate")]
        public DateTime Published { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? Href { get; set; }

        public string? ContentType { get; set; }

        [JsonProperty("htmlbody_Html")]
        public string? HtmlBody { get; set; }

        [JsonProperty("_links")]
        public JObject? Links { get; set; }

        [JsonIgnore]
        public ContentLinksModel? ContentLinks
        {
            get => PrivateLinksModel ??= new ContentLinksModel(Links);

            set => PrivateLinksModel = value;
        }

        [JsonIgnore]
        public IList<IBaseContentItemModel> ContentItems { get; set; } = new List<IBaseContentItemModel>();

        [JsonIgnore]
        private ContentLinksModel? PrivateLinksModel { get; set; }
    }
}
