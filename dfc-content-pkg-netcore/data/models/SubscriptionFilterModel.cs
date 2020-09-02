using Microsoft.Azure.Management.EventGrid.Models;
using System.Collections.Generic;

namespace dfc_content_pkg_netcore.models
{
    public class SubscriptionFilterModel
    {
        public string BeginsWith { get; set; } = "/content/page/";

        public string EndsWith { get; set; }

        public List<string> IncludeEventTypes { get; set; } = new List<string>();

        public StringInAdvancedFilter PropertyContainsFilter { get; set; }
    }
}
