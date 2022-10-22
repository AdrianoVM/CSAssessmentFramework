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
        public List<InspectorOption> movementOptions = new(Enumerable.Repeat(new InspectorOption(), 1));
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

        public void Load()
        {
            JsonSaving.LoadInspectorOptions(movementOptions, "TODO", "SaveData02.dat");
        }


        public void Save()
        {
            JsonSaving.SaveInspectorOptions(movementOptions, "t", "SaveData02.dat");
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
                    new JProperty(nameof(InspectorOption.monoName), opt.monoName),
                    new JProperty(nameof(InspectorOption.MonoType), opt.MonoType?.AssemblyQualifiedName),
                    new JProperty(nameof(InspectorOption.enableOption), opt.enableOption),
                    new JProperty(nameof(InspectorOption.expandOption), opt.expandOption),
                    new JProperty(nameof(InspectorOption.Mono), 
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