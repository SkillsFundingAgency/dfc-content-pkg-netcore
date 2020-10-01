using DFC.Content.Pkg.Netcore.Data.Contracts;
using System;
using System.Collections.Generic;

namespace DFC.Content.Pkg.Netcore.Services
{
    public class ContentTypeMappingService : IContentTypeMappingService
    {
        public Dictionary<string, Type> Mappings { get; } = new Dictionary<string, Type>();

        public List<string> IgnoreRelationship { get; } = new List<string>();

        public void AddIgnoreRelationship(string relationshipName)
        {
            _ = relationshipName ?? throw new ArgumentNullException(nameof(relationshipName));

            if (!IgnoreRelationship.Contains(relationshipName.ToUpperInvariant()))
            {
                IgnoreRelationship.Add(relationshipName.ToUpperInvariant());
            }
        }

        public void RemoveIgnoreRelationship(string relationshipName)
        {
            _ = relationshipName ?? throw new ArgumentNullException(nameof(relationshipName));

            IgnoreRelationship.Remove(relationshipName.ToUpperInvariant());
        }

        public Type? GetMapping(string contentType)
        {
            _ = contentType ?? throw new ArgumentNullException(nameof(contentType));

            if (Mappings.ContainsKey(contentType!.ToUpperInvariant()))
            {
                return Mappings[contentType!.ToUpperInvariant()];
            }

            return null;
        }

        public void AddMapping(string contentType, Type model)
        {
            _ = contentType ?? throw new ArgumentNullException(nameof(contentType));

            if (!Mappings.ContainsKey(contentType.ToUpperInvariant()))
            {
                Mappings.Add(contentType.ToUpperInvariant(), model);
            }

            Mappings[contentType.ToUpperInvariant()] = model;
        }

        public void RemoveMapping(string contentType)
        {
            _ = contentType ?? throw new ArgumentNullException(nameof(contentType));

            if (Mappings.ContainsKey(contentType.ToUpperInvariant()))
            {
                Mappings.Remove(contentType.ToUpperInvariant());
            }
        }
    }
}
