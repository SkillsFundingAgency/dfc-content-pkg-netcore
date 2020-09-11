using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DFC.Content.Pkg.Netcore.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class LinkDetails
    {
        [JsonIgnore]
        public Uri? Uri { get; set; }

        public string? Href { get; set; }

        public string? ContentType { get; set; }

        public string? Alignment { get; set; }

        public int? Ordinal { get; set; }

        public int? Size { get; set; }

        public string? Title { get; set; }
    }
}
