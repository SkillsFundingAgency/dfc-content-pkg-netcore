using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace DFC.Content.Pkg.Netcore.Converters
{
    public class LinkDetailConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ILinkDetails);
        }

        public override void WriteJson(JsonWriter writer,
            object value, JsonSerializer serializer)
        {
            throw new InvalidOperationException("Use default serialization.");
        }

        public override object ReadJson(JsonReader? reader,
            Type objectType, object? existingValue,
            JsonSerializer serializer)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            JsonSerializer se = new JsonSerializer();

            var result = se.Deserialize<LinkDetails>(reader);

            if (result == null)
            {
                throw new InvalidOperationException(nameof(result));
            }

            return result;
        }
    }
}
