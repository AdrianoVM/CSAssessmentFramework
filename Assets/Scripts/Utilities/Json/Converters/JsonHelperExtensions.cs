using System.Globalization;
using Newtonsoft.Json;
using UnityEngine;

namespace Utilities.Json.Converters
{
    /// <summary>
    /// Helper class for Json Conversions
    /// </summary>
    public static class JsonHelperExtensions
    {
        public static float? ReadAsFloat(this JsonReader reader)
        {
            // https://github.com/jilleJr/Newtonsoft.Json-for-Unity.Converters/issues/46

            var str = reader.ReadAsString();

            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            else if (float.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var valueParsed))
            {
                return valueParsed;
            }
            else
            {
                return 0f;
            }
        }
        
        public static void ReplaceValueWithID<T>(ref T param, string id, string objectName) where T : class
        {
            // necessary for outside editor
            // ReSharper disable once RedundantAssignment
            T asset = null;
#if UNITY_EDITOR
            asset = FindObjects.FindInAssets(id, typeof(T)) as T;
#endif
            if (asset != null)
            {
                param = asset;
            }
            else
            {
                if (id != "")
                {
                    Debug.LogWarning("Value of " + nameof(param) + " from " + objectName +
                                     "was not replaced as the Asset was not found");
                }
                else
                {
                    param = null;
                }
            }
        }
    }
}