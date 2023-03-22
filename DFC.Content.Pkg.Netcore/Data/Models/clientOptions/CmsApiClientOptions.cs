using System.Diagnostics.CodeAnalysis;

namespace DFC.Content.Pkg.Netcore.Data.Models.ClientOptions
{
    [ExcludeFromCodeCoverage]
    public class CmsApiClientOptions : ClientOptionsModel
    {
        public string SummaryEndpoint { get; set; } = "/page";

        public string StaticContentEndpoint { get; set; } = "/sharedcontent/";

        public string? ContentIds { get; set; }
    }
}
