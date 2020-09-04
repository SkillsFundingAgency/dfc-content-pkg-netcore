using System.Diagnostics.CodeAnalysis;

namespace dfc_cmsapi_pkg_netcore.Data.models
{
    [ExcludeFromCodeCoverage]
    public class PolicyOptions
    {
        public CircuitBreakerPolicyOptions HttpCircuitBreaker { get; set; } = new CircuitBreakerPolicyOptions();

        public RetryPolicyOptions HttpRetry { get; set; } = new RetryPolicyOptions();
    }
}
