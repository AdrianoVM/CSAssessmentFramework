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
    public class MonoContractResolver : UnityTypeContractResolver
    {
        public new static readonly MonoContractResolver Instance = new MonoContractResolver();
        

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
                        //property.ValueProvider = new MonoValueProvider(property.ValueProvider, property.PropertyName);
                        property.Converter = new MonoConverter();
                    }
                }
            }
            else
            {
                property.ShouldSerialize = o => false;
            }
            
            return property;
        }
    }
    
}