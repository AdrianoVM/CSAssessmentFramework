using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Learning;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace Utilities
{
    public class MonoContractResolver : DefaultContractResolver
    {
        public new static readonly MonoContractResolver Instance = new MonoContractResolver();

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);
            // Only filter class that is derived from MonoBehaviour
            if (type.IsSubclassOf(typeof(MonoBehaviour)) && type.GetInterface(nameof(ISerializable)) == null)
            {
                // Keep name property OR properties derived from MonoBehaviour
                //properties = properties.Where(x => x.PropertyName.Equals("name") || x.DeclaringType.IsSubclassOf(typeof(MonoBehaviour)) ).ToList();
                //properties = properties.Where(x => x.PropertyName.Equals("name") ).ToList();
                properties = null;
            }

            if (type == typeof(Material))
            {
                properties = null;
            }
            return properties;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            return base.CreateProperty(member, memberSerialization);
        }
    }
    
}