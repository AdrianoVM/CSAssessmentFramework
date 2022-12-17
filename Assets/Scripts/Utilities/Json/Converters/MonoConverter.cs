using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities.Json.Converters
{
    /// <summary>
    /// Converts MonoBehaviours contained in Managers to and from a specific JSON format.
    /// Handles references and list of references. Finding the corresponding object when reading,
    /// and saving the reference when writing.
    /// </summary>
    public class MonoConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            
            List<Object> listOfObj;
            var array = new JArray();
            var isList = value.GetType().IsGenericType && value is IEnumerable;
            if (isList)
            {
                listOfObj = value.ConvertTo<List<Object>>();
            }
            else
            {
                listOfObj = new List<Object>();
                listOfObj.Add((Object)value);
            }

            
            foreach (Object obj in listOfObj)
            {
                var path = "";
                var isAsset = false;
                if (obj != null)
                {
                    // If it is in the scene
                    bool isP;
#if UNITY_EDITOR
                    isP = !EditorUtility.IsPersistent(obj);
#else
                    isP = false;
#endif
                    if (obj.GetType().IsSubclassOf(typeof(Component)) && isP)
                    {
                        path = obj.GetInstanceID().ToString();
                    }
                    else
                    {
#if UNITY_EDITOR
                        isAsset = true;
                        path = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj));
#endif
                    }
                }
                var objInfo = new ObjectID(path, isAsset, obj != null ? obj.GetType() : null, obj != null ? obj.name : null);
                array.Add(JObject.FromObject(objInfo));
            }

            if (isList)
            {
                array.WriteTo(writer);
            }
            else
            {
                var jo = (JObject)array.First;
                jo?.WriteTo(writer);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var arr = new JArray();
            
            var isList = reader.TokenType == JsonToken.StartArray;
            //Making a list of the correct type or we get an error
            Type makeMe;
            if (!isList)
            {
                Type d1 = typeof(List<>);
                Type[] typeArgs = { objectType };
                makeMe = d1.MakeGenericType(typeArgs);
            }
            else
            {
                makeMe = objectType;
            }
            

            var existingObjs = (IList)Activator.CreateInstance(makeMe);
            var objectList = (IList)Activator.CreateInstance(makeMe);
            if (isList)
            {
                arr = JArray.Load(reader);
                existingObjs = (IList)existingValue.ConvertTo(makeMe);
            }
            else
            {
                JObject obj = JObject.Load(reader);
                arr.Add(obj);
                existingObjs.Add(existingValue);
            }
            

            foreach ((JToken token, var idx) in arr.WithIndex())
            {
                var readId = token.ToObject<ObjectID>();
                object found;
                
#if UNITY_EDITOR
                found = ObjectID.FindObjectByID(readId);
#else
                found = null;
#endif
                if (existingObjs.Count <= idx)
                {
                    objectList.Add(found);
                }
                else
                {
                    objectList.Add(found ?? existingObjs[idx]);
                }
            }

            if (isList)
            {
                
                return objectList;
            }
            else
            {
                return objectList[0];
            }

        }
        
        public override bool CanWrite => true;

        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            // We need to make it false as only specific instances of Objects use this converter.
            return false;
        }
    }
}