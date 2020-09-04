using System.Diagnostics.CodeAnalysis;

namespace dfc_cmsapi_pkg_netcore.Data.models
{
    [ExcludeFromCodeCoverage]
    public class RetryPolicyOptions
    {
        public int Count { get; set; } = 3;

        public int BackoffPower { get; set; } = 2;
    }
}
