using System;

namespace DFC.Content.Pkg.Netcore.Data.Contracts
{
    public interface ILinkDetails
    {
        Uri? Uri { get; set; }

        string? Href { get; set; }

        string? ContentType { get; set; }

        string? Alignment { get; set; }

        int? Ordinal { get; set; }

        int? Size { get; set; }

        string? Title { get; set; }
    }
}
