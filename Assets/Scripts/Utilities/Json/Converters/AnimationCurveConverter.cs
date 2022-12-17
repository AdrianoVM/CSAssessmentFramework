using Newtonsoft.Json;
using Newtonsoft.Json.UnityConverters;
using UnityEngine;

namespace Utilities.Json.Converters
{
    /// <summary>
    /// Converts <see cref="AnimationCurve"/> from and into JSON 
    /// </summary>
    public class AnimationCurveConverter : PartialConverter<AnimationCurve>
    {
        protected override void ReadValue(ref AnimationCurve value, string name, JsonReader reader, JsonSerializer serializer)
        {
            switch (name)
            {
                case nameof(value.keys):
                    var jsonString = reader.ReadAsString();
                    if (jsonString != null && jsonString != "")
                    {
                        value.keys = JsonConvert.DeserializeObject<Keyframe[]>(jsonString);
                    }
                    break;
            }
        }

        protected override void WriteJsonProperties(JsonWriter writer, AnimationCurve value, JsonSerializer serializer)
        {
            writer.WritePropertyName(nameof(value.keys));
            writer.WriteValue(JsonConvert.SerializeObject(value.keys));
        }
    }
}