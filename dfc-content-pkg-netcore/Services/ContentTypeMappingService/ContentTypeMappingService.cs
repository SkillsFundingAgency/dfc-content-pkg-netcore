using DFC.Content.Pkg.Netcore.Data.Contracts;
using System;
using System.Collections.Generic;

namespace DFC.Content.Pkg.Netcore.Services
{
    public class ContentTypeMappingService : IContentTypeMappingService
    {
        public Dictionary<string, Type> Mappings { get; } = new Dictionary<string, Type>();

        public Type? GetMapping(string contentType)
        {
            if (Mappings.ContainsKey(contentType))
            {
                return Mappings[contentType];
            }

            return null;
        }

        public void AddMapping(string contentType, Type model)
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
