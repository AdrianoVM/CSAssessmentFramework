using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using Utilities;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Options
{
    public class JsonSaving : Object
    {
        private static JsonSerializerSettings _settings = new()
        {
            ContractResolver = new MonoContractResolver()
        };

        /// <summary>
        /// Saves the <c>InspectorOptions</c> in <paramref name="options"/> as a JSON file.
        /// </summary>
        /// <param name="options">The list of <c>InspectorOptions</c> which need to be saved</param>
        /// <param name="name">The name of the Manager trying to save</param>
        /// <param name="filename">The fileName in which to save the data</param>
        public static void SaveInspectorOptions(List<InspectorOption> options, string name, string filename)
        {
            var rss = new JObject();
            
            // Temporary placement of MonoType refresh:
            foreach (InspectorOption opt in options)
            {
                if (opt.Mono != null)
                {
                    opt.MonoType = opt.Mono.GetType();
                }
                
            }
            var toSave = new JProperty(name,
                new JArray(from opt in options
                    select new JObject(
                        new JProperty(nameof(InspectorOption.monoName), opt.monoName),
                        new JProperty(nameof(InspectorOption.MonoType), opt.MonoType?.AssemblyQualifiedName),
                        new JProperty(nameof(InspectorOption.EnableOption), opt.EnableOption),
                        new JProperty(nameof(InspectorOption.expandOption), opt.expandOption),
                        new JProperty(nameof(InspectorOption.Mono),
                            opt.Mono != null
                                ? JObject.Parse(JsonConvert.SerializeObject(opt.Mono, _settings))
                                : null))));
            if (FileManager.LoadFromFile(filename, out var json, true, true))
            {
                rss = JObject.Parse(json);
                if (rss.ContainsKey(name))
                {
                    rss[name]?.Replace(toSave.Value);
                }
                else
                {
                    rss.Add(toSave);
                }
            }
            else
            {
                rss.Add(toSave);
            }
            
            if (FileManager.WriteToFile(filename, rss.ToString(), true))
            {
                Debug.Log("Save successful");
            }
        }
        
        
        /// <summary>
        /// Loads the <c>InspectorOption</c> into the <paramref name="options"/> parameter.
        /// </summary>
        /// <param name="options">The list of <c>InspectorOptions</c> which need to be saved</param>
        /// <param name="name">The name of the Manager trying to load</param>
        /// <param name="filename">The fileName from which to load the data</param>
        public static void LoadInspectorOptions(List<InspectorOption> options, string name, string filename)
        {
            JObject rss;

            if (FileManager.LoadFromFile(filename, out var json, true))
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                JArray fullList;
                rss = JObject.Parse(json);
                if (rss.ContainsKey(name))
                {
                    fullList = rss[name]?.Value<JArray>();
                }
                else
                {
                    Debug.Log("Nothing to load for "+name);
                    stopWatch.Stop();
                    return;
                }
                
                
                foreach ((JToken optionToken, var i) in fullList.WithIndex())
                {
                    DeSerializeOption(options, i, optionToken);
                }

                
                var loadedListSize = fullList != null ? fullList.Count : 0;
                // disables the monoBehaviours that are going to be removed
                for (var i = loadedListSize; i < options.Count; i++)
                {
                    if (options[i].Mono != null)
                    {
                        options[i].Mono.enabled = false;
                    }
                }
                // Removes the MonoBehaviours that are not present in the loaded list
                if (loadedListSize < options.Count)
                {
                    options.RemoveRange(loadedListSize,options.Count-loadedListSize);
                }
                stopWatch.Stop();
                Debug.Log("Load successful in: " + stopWatch.Elapsed.ToString(@"m\:ss\.fff"));
            }
            
        }
        
        //TODO: issues with some classes (Material), how to search for stuff outside of scene?
        //TODO: Make it so that it doesn't overwrite if the ref is dead?
        private static void DeSerializeOption(List<InspectorOption> options, int i, JToken optionToken)
        {
            if (options.Count <= i)
            {
                options.Add(new InspectorOption());
            }
            var nameToken = optionToken[nameof(InspectorOption.monoName)]?.ToString();
            if (options[i].monoName != nameToken)
            {
                options.Insert(i,new InspectorOption());
            }

            // Finding MonoBehaviour and Updating It.
            var monoTypeToken = optionToken[nameof(InspectorOption.MonoType)]?.ToObject<Type>();
            if (monoTypeToken != null)
            {
                options[i].MonoType = monoTypeToken;
                //Searching for the MonoBehaviour 
                if (options[i].Mono == null || options[i].Mono.GetType() != monoTypeToken)
                {
                    MonoBehaviour found = (MonoBehaviour)FindObjects.FindInScene(monoTypeToken, nameToken) ?? options[i].Mono;
                    // for when there are multiple objects of same type in JSON but less in scene.
                    for (var j = 0; j < i; j++)
                    {
                        if (options[j].Mono == found)
                        {
                            found = null;
                        }
                    }

                    options[i].Mono = found;
                }
                // Replacing values in MonoBehaviour if it exists
                if (options[i].Mono != null)
                {
                    JsonConvert.PopulateObject(optionToken[nameof(InspectorOption.Mono)].ToString(), options[i].Mono, _settings);
                    //JsonUtility.FromJsonOverwrite(optionToken[nameof(InspectorOption.Mono)].ToString(), options[i].Mono);
                }
                else
                {
                    //Adding name to InspectorOption if no MonoBehaviour are found
                    options[i].monoName = nameToken;
                    options[i].MonoType = monoTypeToken;
                }
                
            }
            //updating the parameters of each InspectorOption
            var enableToken = optionToken[nameof(InspectorOption.EnableOption)]?.ToObject<bool>();
            options[i].EnableOption = enableToken != null ? (bool)enableToken : false;
            var expandToken = optionToken[nameof(InspectorOption.expandOption)]?.ToObject<bool>();
            options[i].expandOption = expandToken != null ? (bool)expandToken : false;

        }
    }
    
    


    public class ObjectID
    {
        public string ID;
        public bool IsAsset;
        public Type ObjectType;
        public string ObjectName;


        public ObjectID(string id, bool isAsset,Type objectType, string objectName)
        {
            ID = id;
            IsAsset = isAsset;
            ObjectType = objectType;
            ObjectName = objectName;
        }

        public static object FindObjectByID(ObjectID objectID)
        {
            Object o = null;
            Type t = objectID.ObjectType;
            if (objectID.ID != "")
            {
                if (objectID.IsAsset)
                {
                    o = FindObjects.FindInAssets(objectID.ID, t);
                }
                else
                {
                    o = EditorUtility.InstanceIDToObject(Convert.ToInt32(objectID.ID));
                    if (!o || objectID.ObjectType != o.GetType())
                    {
                        o = FindObjects.FindInScene(t, objectID.ObjectName);
                    }
                }
            }
            return o;
        }
    }
}