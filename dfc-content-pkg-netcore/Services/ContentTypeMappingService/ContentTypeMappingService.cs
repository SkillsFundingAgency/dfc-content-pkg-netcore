using DFC.Content.Pkg.Netcore.Data.Contracts;
using System.Collections.Generic;

namespace DFC.Content.Pkg.Netcore.Services
{
    public class ContentTypeMappingService : IContentTypeMappingService
    {
        public Dictionary<string, IBaseContentItemModel> Mappings { get; } = new Dictionary<string, IBaseContentItemModel>();

        public void AddMapping(string contentType, IBaseContentItemModel model)
        {
            if (!Mappings.ContainsKey(contentType))
            {
                Mappings.Add(contentType, model);
            }

            Mappings[contentType] = model;
        }

        public void RemoveMapping(string contentType)
        {
            if (Mappings.ContainsKey(contentType))
            {
                Mappings.Remove(contentType);
            }
        }
    }
}
