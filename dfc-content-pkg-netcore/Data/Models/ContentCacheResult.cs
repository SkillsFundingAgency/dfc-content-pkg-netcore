using DFC.Content.Pkg.Netcore.Data.Enums;

namespace DFC.Content.Pkg.Netcore.Data.Models
{
    public class ContentCacheResult
    {
        public string? ContentType { get; set; }

        public ContentCacheStatus Result { get; set; }
    }
}
