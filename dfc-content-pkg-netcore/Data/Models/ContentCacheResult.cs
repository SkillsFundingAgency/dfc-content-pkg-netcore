using DFC.Content.Pkg.Netcore.Data.Enums;
using System;

namespace DFC.Content.Pkg.Netcore.Data.Models
{
    public class ContentCacheResult
    {
        public string? ContentType { get; set; }

        public ContentCacheStatus Result { get; set; }

        public Guid? ParentContentId { get; set; }
    }
}
