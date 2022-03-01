using System;

namespace DFC.Content.Pkg.Netcore.Data.Models
{
    public class CacheLookupOptions
    {
        public string? KeyName { get; set; }

        public Func<string, string>? Transform { get; set; }
    }
}