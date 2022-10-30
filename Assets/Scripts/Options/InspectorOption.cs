using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Options
{
    [Serializable] //[JsonObject(MemberSerialization.OptIn)]
    public class InspectorOption
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
                    _monoType = value.GetType();
                }
            }
        }

        public bool EnableOption
        {
            get
            {
                if (Mono != null)
                {
                    enableOption = Mono.enabled;
                    return enableOption;
                }
                return enableOption;
            }
            set
            {
                if (Mono != null)
                {
                    Mono.enabled = value;
                } 
                enableOption = value;
            }
        }

        public Type MonoType
        {
            get
            {
                if (_monoType == null && MonoTypeName != "")
                {
                    _monoType = Type.GetType(MonoTypeName);
                }
                return _monoType;
            }
            set
            {
                _monoType = value;
                MonoTypeName = value == null ? "" : value.AssemblyQualifiedName;
            }
        }

        public string monoName = "New Option";
        public Type _monoType;
        public string MonoTypeName = "";
        [SerializeField]
        private bool enableOption = true;
        public bool expandOption = true;

        public InspectorOption()
        {
            monoBehaviour = null;
        }

        public InspectorOption(MonoBehaviour mono)
        {
            monoBehaviour = mono;
        }

        public void Reset()
        {
            monoName = "New Option";
            _monoType = null;
        }

        
    }
}