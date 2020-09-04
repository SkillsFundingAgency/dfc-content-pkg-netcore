using System;
using System.Diagnostics.CodeAnalysis;

namespace dfc_cmsapi_pkg_netcore.Data.models
{
    [ExcludeFromCodeCoverage]
    public class CircuitBreakerPolicyOptions
    {
        public TimeSpan DurationOfBreak { get; set; } = TimeSpan.FromSeconds(30);

        public int ExceptionsAllowedBeforeBreaking { get; set; } = 12;
    }
}
