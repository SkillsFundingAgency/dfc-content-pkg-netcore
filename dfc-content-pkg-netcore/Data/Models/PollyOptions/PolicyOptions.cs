using System.Diagnostics.CodeAnalysis;

namespace DFC.Content.Pkg.Netcore.Data.Models.PollyOptions
{
    [ExcludeFromCodeCoverage]
    public class PolicyOptions
    {
        public CircuitBreakerPolicyOptions HttpCircuitBreaker { get; set; } = new CircuitBreakerPolicyOptions();

        public RetryPolicyOptions HttpRetry { get; set; } = new RetryPolicyOptions();
    }
}
