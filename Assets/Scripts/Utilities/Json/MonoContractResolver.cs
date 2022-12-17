using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.UnityConverters;
using UnityEngine;
using Utilities.Json.Converters;
using Object = UnityEngine.Object;

namespace Utilities.Json
{
    /// <summary>
    /// Contract resolver used to specify the <see cref="JsonConverter"/> of components saved in Managers
    /// </summary>
    public class MonoContractResolver : UnityTypeContractResolver
    {
        public static readonly MonoContractResolver Instance = new();
        

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property!.DeclaringType != null && property.DeclaringType.IsSubclassOf(typeof(MonoBehaviour)))
            {
                if (property.PropertyType != null)
                {
                    var listType = property.PropertyType.GetGenericArguments();

                    if (property.PropertyType.IsSubclassOf(typeof(Object)) || (listType.Length > 0 && listType.Single().IsSubclassOf(typeof(Object))))
                    {
                        property.Converter = new MonoConverter();
                    }
                }
            }
            else
            {
                property.ShouldSerialize = _ => false;
            }
            
            return property;
        }
    }
    
}