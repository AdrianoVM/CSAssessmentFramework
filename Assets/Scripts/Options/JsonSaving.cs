using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Utilities;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Options
{
    public class JsonSaving : Object
    {
        //private static JObject rss;
        public static void SaveInspectorOptions(List<InspectorOption> options, string name, string filename)
        {
            JObject rss = new JObject();
            
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
                        new JProperty(nameof(InspectorOption.enableOption), opt.enableOption),
                        new JProperty(nameof(InspectorOption.expandOption), opt.expandOption),
                        new JProperty(nameof(InspectorOption.Mono),
                            opt.Mono != null
                                ? JObject.Parse(JsonUtility.ToJson(opt.Mono, prettyPrint: true))
                                : null))));
            if (FileManager.LoadFromFile(filename, out var json))
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
            
            if (FileManager.WriteToFile(filename, rss.ToString()))
            {
                Debug.Log("Save successful");
            }
        }
        
        
        
        public static void LoadInspectorOptions(List<InspectorOption> options, string name, string filename)
        {
            JObject rss;

            if (FileManager.LoadFromFile("SaveData02.dat", out var json))
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

                if (fullList?.Count < options.Count)
                {
                    options.RemoveRange(fullList.Count,options.Count-fullList.Count);
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
            //updating the parameters of each InspectorOption
            var enableToken = optionToken[nameof(InspectorOption.enableOption)]?.ToObject<bool>();
            options[i].enableOption = enableToken != null ? (bool)enableToken : false;
            var expandToken = optionToken[nameof(InspectorOption.expandOption)]?.ToObject<bool>();
            options[i].expandOption = expandToken != null ? (bool)expandToken : false;

            // Finding MonoBehaviour and Updating It.
            //var monoTypeToken = optionToken[nameof(InspectorOption.Mono)]?["$type"]?.ToObject<Type>();
            var monoTypeToken = optionToken[nameof(InspectorOption.MonoType)]?.ToObject<Type>();
            if (monoTypeToken != null)
            {
                options[i].MonoType = monoTypeToken;
                //Searching for the MonoBehaviour 
                if (options[i].Mono == null || options[i].Mono.GetType() != monoTypeToken)
                {
                    
                    var foundObjects = FindObjectsOfType(monoTypeToken, true);
                    // TODO: add condition if there is only one, disregard name
                    foreach (Object foundObject in foundObjects)
                    {
                        if (foundObject.name == nameToken)
                        {
                            options[i].Mono = (MonoBehaviour)foundObject;
                            break;
                        }
                    }
                }
                // Replacing values in MonoBehaviour if it exists
                if (options[i].Mono != null)
                {
                    //JsonConvert.PopulateObject(optionToken[nameof(InspectorOption.Mono)].ToString(), options[i].Mono);
                    JsonUtility.FromJsonOverwrite(optionToken[nameof(InspectorOption.Mono)].ToString(), options[i].Mono);
                }
                else
                {
                    //Adding name to InspectorOption if no MonoBehaviour are found
                    options[i].monoName = nameToken;
                    options[i].MonoType = monoTypeToken;
                }
                
            }
        }
    }
}