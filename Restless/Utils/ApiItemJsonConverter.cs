using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Restless.Models;

namespace Restless.Utils
{
    public class ApiItemJsonConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var json = JObject.Load(reader);
            var type = (ApiItemType)JsonConvert.DeserializeObject(json.GetValue(nameof(ApiItem.Type)).ToString(Formatting.Indented), typeof(ApiItemType));
            ApiItem item;
            switch (type)
            {
                case ApiItemType.Api:
                    item = new Api();
                    break;
                case ApiItemType.Collection:
                    item = new ApiCollection();
                    break;
                default:
                    throw new Exception();
            }

            serializer.Populate(json.CreateReader(), item);
            return item;
        }

        public override bool CanConvert(Type objectType) => typeof(ApiItem).IsAssignableFrom(objectType);
    }
}