using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.Content.Pkg.Netcore.Data.Contracts
{
    public interface IContentTypeMappingService
    {
        Dictionary<string, IBaseContentItemModel> Mappings { get; }

        void AddMapping(string contentType, IBaseContentItemModel model);

        void RemoveMapping(string contentType);
    }
}
