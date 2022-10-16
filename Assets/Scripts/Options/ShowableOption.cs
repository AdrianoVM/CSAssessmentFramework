using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Options
{
    [Serializable] //[JsonObject(MemberSerialization.OptIn)]
    public class ShowableOption
    {
        [SerializeField]
        private MonoBehaviour monoBehaviour;
        public MonoBehaviour Mono
        {
            get => monoBehaviour;
            set 
            { 
                monoBehaviour = value;
                if (value != null)
                {
                    monoName = value.name;
                    MonoType = value.GetType();
                }
            }
        }
        //[JsonProperty]
        public string monoName = "New Option";
        public Type MonoType;
        public bool enableOption = true;
        public bool expandOption = true;

        public ShowableOption()
        {
            monoBehaviour = null;
        }

        public ShowableOption(MonoBehaviour mono)
        {
            monoBehaviour = mono;
        }

        public void Reset()
        {
            monoName = "New Option";
            MonoType = null;
        }

        
    }
}