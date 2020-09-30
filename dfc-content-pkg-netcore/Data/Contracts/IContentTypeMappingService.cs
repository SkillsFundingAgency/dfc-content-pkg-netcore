using System.Collections.Generic;

namespace DFC.Content.Pkg.Netcore.Data.Contracts
{
    public interface IContentTypeMappingService
    {
        Dictionary<string, IBaseContentItemModel> Mappings { get; }

        IBaseContentItemModel? GetMapping(string contentType);

        void AddMapping(string contentType, IBaseContentItemModel model);

        void RemoveMapping(string contentType);
    }
}
