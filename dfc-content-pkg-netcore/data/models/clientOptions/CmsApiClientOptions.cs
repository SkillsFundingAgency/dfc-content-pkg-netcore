namespace dfc_content_pkg_netcore.models.clientOptions
{
    public class CmsApiClientOptions : ClientOptionsModel
    {
        public string SummaryEndpoint { get; set; } = "content/getcontent/api/execute/page";

        public string StaticContentEndpoint { get; set; } = "content/getcontent/api/execute/sharedcontent/";

        public string ContentIds { get; set; }
    }
}
