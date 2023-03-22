using System.Collections.Generic;

namespace DFC.Content.Pkg.Netcore.Data.Models
{
    public class CmsApiOptions
    {
        public bool PreventRecursion { get; set; }

        public Dictionary<string, CacheLookupOptions> ContentTypeOptions { get; set; } = new Dictionary<string, CacheLookupOptions>();
    }
}