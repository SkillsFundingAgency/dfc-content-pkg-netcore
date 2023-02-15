using System.Diagnostics.CodeAnalysis;

namespace DFC.Content.Pkg.Netcore.Data.Models.PollyOptions
{
    [ExcludeFromCodeCoverage]
    public class RetryPolicyOptions
    {
        public int Count { get; set; } = 3;

        public int BackoffPower { get; set; } = 2;
    }
}
