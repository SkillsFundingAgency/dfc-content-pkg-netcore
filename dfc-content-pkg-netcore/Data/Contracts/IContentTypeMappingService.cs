using System;
using System.Collections.Generic;

namespace DFC.Content.Pkg.Netcore.Data.Contracts
{
    public interface IContentTypeMappingService
    {
        Dictionary<string, Type> Mappings { get; }

        Type? GetMapping(string contentType);

        void AddMapping(string contentType, Type model);

        void RemoveMapping(string contentType);
    }
}
