using System;
using Newtonsoft.Json;
using UnityEngine;
using Utilities;

namespace Learning
{
    //[JsonObject(MemberSerialization.OptIn)]
    public class BasicMono : MonoBehaviour
    {
        [SerializeField] private int count;
        [SerializeField] private MonoBehaviour test; // need public or [JsonProperty]
        public string message = "no";
        [SerializeField] private Material material;
        private void OnEnable()
        {
            Debug.Log(count);
        }
        
        public void OnLoad()
        {
            
            if (FileManager.LoadFromFile("SaveData01.dat", out var json))
            {
                JsonUtility.FromJsonOverwrite(json, this);
            }
        }
        public void OnSave()
        {
           string fileContent = JsonUtility.ToJson(this, prettyPrint: true);
            if (FileManager.WriteToFile("SaveData01.dat", fileContent))
            {
                Debug.Log("Save successful");
            }
        }
        
    }
}