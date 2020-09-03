namespace dfc_content_pkg_netcore.models
{
    public class EventGridSubscriptionModel
    {
        public string Name { get; set; }

        public string Endpoint { get; set; }

        public SubscriptionFilterModel Filter { get; set; } = new SubscriptionFilterModel();
    }
}
