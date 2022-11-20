using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.UnityConverters;
using PathCreation;
using UnityEngine;

namespace Utilities.Json.Converters
{
    /// <summary>
    /// Converts BezierPaths from the spline asset we use, will break once we use another spline asset.
    /// </summary>
    public class BezierPathConverter : PartialConverter<BezierPath>
    {
        protected override void ReadValue(ref BezierPath value, string name, JsonReader reader, JsonSerializer serializer)
        {
            
            switch (name)
            {
                case "pointList":
                    var jsonString = reader.ReadAsString();
                    var list = JsonConvert.DeserializeObject<List<Vector3>>(jsonString ?? string.Empty);

                    value = new BezierPath(list, value.IsClosed, value.Space);
                    break;
                case nameof(value.IsClosed):
                    value.IsClosed = reader.ReadAsBoolean() ?? false;
                    break;
                case nameof(value.Space):
                    PathSpace t;
                    if (Enum.TryParse(reader.ReadAsString() ?? "", out t))
                    {
                        value.Space = t;
                    }
                    break;
            }
        }

        protected override void WriteJsonProperties(JsonWriter writer, BezierPath value, JsonSerializer serializer)
        {
            writer.WritePropertyName(nameof(value.IsClosed));
            writer.WriteValue(value.IsClosed);
            writer.WritePropertyName(nameof(value.Space));
            writer.WriteValue(value.Space);
            writer.WritePropertyName("pointList");
            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i < value.NumPoints; i+=3)
            {
                points.Add(value.GetPoint(i));
            }
            writer.WriteValue(JsonConvert.SerializeObject(points));
        }
    }
}