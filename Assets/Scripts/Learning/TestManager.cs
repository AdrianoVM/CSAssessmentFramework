using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Options;
using UnityEngine;
using Utilities;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Learning
{
    public class TestManager : MonoBehaviour
    {
        [SerializeField] private ScriptableTest saveFile;
        public List<ShowableOption> movementOptions = new(Enumerable.Repeat(new ShowableOption(), 1));
        [SerializeField] private int testInt;
        [SerializeField] public BasicMono testMono;

        public void OnLoad()
        {
            if (saveFile != null)
            {
                testInt = saveFile.testInt;
                testMono = (BasicMono) saveFile.testMono;
            }
            if (FileManager.LoadFromFile("SaveData01.dat", out var json))
            {
                JsonUtility.FromJsonOverwrite(json, this);
            }
        }
        
        public void OnJsonLoad()
        {
            // if (FileManager.LoadFromFile("SaveData02.dat", out var json))
            // {
            //     JsonUtility.FromJsonOverwrite(json, this);
            // }

            if (FileManager.LoadFromFile("SaveData02.dat", out var json))
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                JArray fullList = JArray.Parse(json);
                
                foreach ((JToken optionToken, var i) in fullList.WithIndex())
                {
                    DeSerializeOption(i, optionToken);
                }

                if (fullList.Count < movementOptions.Count)
                {
                    movementOptions.RemoveRange(fullList.Count,movementOptions.Count-fullList.Count);
                }
                stopWatch.Stop();
                Debug.Log("Load successful in: " + stopWatch.Elapsed.ToString(@"m\:ss\.fff"));
            }
            
            

        }

        //TODO: issues with some classes (Material), how to search for stuff outside of scene?
        private void DeSerializeOption(int i, JToken optionToken)
        {
            if (movementOptions.Count <= i)
            {
                movementOptions.Add(new ShowableOption());
            }
            var nameToken = optionToken[nameof(ShowableOption.monoName)]?.ToString();
            if (movementOptions[i].monoName != nameToken)
            {
                movementOptions.Insert(i,new ShowableOption());
            }
            //updating the parameters of each ShowableOption
            var enableToken = optionToken[nameof(ShowableOption.enableOption)]?.ToObject<bool>();
            movementOptions[i].enableOption = enableToken != null ? (bool)enableToken : false;
            var expandToken = optionToken[nameof(ShowableOption.expandOption)]?.ToObject<bool>();
            movementOptions[i].expandOption = expandToken != null ? this : false;

            // Finding Monobehaviour and Updating It.
            //var monoTypeToken = optionToken[nameof(ShowableOption.Mono)]?["$type"]?.ToObject<Type>();
            var monoTypeToken = optionToken[nameof(ShowableOption.MonoType)]?.ToObject<Type>();
            if (monoTypeToken != null)
            {
                movementOptions[i].MonoType = monoTypeToken;
                //Searching for the monobehaviour 
                if (movementOptions[i].Mono == null || movementOptions[i].Mono.GetType() != monoTypeToken)
                {
                    
                    var foundObjects = FindObjectsOfType(monoTypeToken, true);
                    // TODO: add condition if there is only one, disregard name
                    foreach (Object foundObject in foundObjects)
                    {
                        if (foundObject.name == nameToken)
                        {
                            movementOptions[i].Mono = (MonoBehaviour)foundObject;
                            break;
                        }
                    }
                }
                // Replacing values in MonoBehaviour if it exists
                if (movementOptions[i].Mono != null)
                {
                    //JsonConvert.PopulateObject(optionToken[nameof(ShowableOption.Mono)].ToString(), movementOptions[i].Mono);
                    JsonUtility.FromJsonOverwrite(optionToken[nameof(ShowableOption.Mono)].ToString(), movementOptions[i].Mono);
                }
                else
                {
                    //Adding name to ShowableOption if no monobehaviour are found
                    movementOptions[i].monoName = nameToken;
                    movementOptions[i].MonoType = monoTypeToken;
                }
                
            }
        }


        public void OnSave()
        {
            if (saveFile != null)
            {

                saveFile.testInt = testInt;
                saveFile.testMono = testMono;
            }
            string fileContent = JsonUtility.ToJson(this, prettyPrint: true);
            if (FileManager.WriteToFile("SaveData01.dat", fileContent))
            {
                Debug.Log("Save successful");
            }
        }

        public void OnJsonSave()
        {
            JArray ttt = new JArray(from opt in movementOptions
                select new JObject(
                    new JProperty(nameof(ShowableOption.monoName), opt.monoName),
                    new JProperty(nameof(ShowableOption.MonoType), opt.MonoType?.AssemblyQualifiedName),
                    new JProperty(nameof(ShowableOption.enableOption), opt.enableOption),
                    new JProperty(nameof(ShowableOption.expandOption), opt.expandOption),
                    new JProperty(nameof(ShowableOption.Mono), 
                        opt.Mono != null ? JObject.Parse(JsonUtility.ToJson(opt.Mono, prettyPrint: true)) : null)));
            if (FileManager.WriteToFile("SaveData02.dat", ttt.ToString()))
            {
                Debug.Log("Save successful");
            }
            return;
            
            string fileContent = JsonConvert.SerializeObject(movementOptions, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new MonoContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto
            });
            //string fileContent = JsonUtility.ToJson(this, prettyPrint: true);
            if (FileManager.WriteToFile("SaveData02.dat", fileContent))
            {
                Debug.Log("Save successful");
            }
            
        }
    }
}