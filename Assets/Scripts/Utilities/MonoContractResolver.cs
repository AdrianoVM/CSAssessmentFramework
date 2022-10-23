using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Learning;
using Mono.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.UnityConverters;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities
{
    public class MonoContractResolver : UnityTypeContractResolver
    {
        public new static readonly MonoContractResolver Instance = new MonoContractResolver();

        /*
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);
            // Only filter class that is derived from MonoBehaviour
            if (type.IsSubclassOf(typeof(MonoBehaviour)))
            {
                // Keep name property OR properties derived from MonoBehaviour
                //properties = properties.Where(x => x.PropertyName.Equals("name") || x.DeclaringType.IsSubclassOf(typeof(MonoBehaviour)) ).ToList();
                //properties = properties.Where(x => x.PropertyName.Equals("name") ).ToList();
                //properties = null;
                IList<JsonProperty> newList = new List<JsonProperty>();
                foreach (JsonProperty property in properties)
                {
                    if (property.DeclaringType != null && property.PropertyType != null && property.DeclaringType.IsSubclassOf(typeof(MonoBehaviour)))
                    {
                        if (!property.PropertyType.IsSubclassOf(typeof(Object)))
                        {
                            newList.Add(property);
                        }
                        else
                        {
                            //property.ValueProvider = new MonoValueProvider(property);
                        }
                    }
                    
                }

                properties = newList;
            }

            // if (type == typeof(Material))
            // {
            //     properties = null;
            // }
            return properties;
        }
        */

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