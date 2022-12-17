using System;
using UnityEngine;

namespace Options
{
    /// <summary>
    /// Class used to encapsulate components stored in a <c>Manager</c>, facilitating serialization.
    /// </summary>
    [Serializable]
    public class InspectorOption
    {
        [SerializeField] private MonoBehaviour monoBehaviour;
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

        [SerializeField] private bool enableOption = true;

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

        private Type _monoType;


        public Type MonoType
        {
            get
            {
                if (_monoType == null && monoTypeName != "")
                {
                    _monoType = Type.GetType(monoTypeName);
                }
                return _monoType;
            }
            set
            {
                _monoType = value;
                monoTypeName = value == null ? "" : value.AssemblyQualifiedName;
            }
        }

        public string monoName = "New Option";
        public string monoTypeName = "";
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

        /// <summary>
        /// When <see cref="monoBehaviour"/> is updated in inspector the set part is not handled.
        /// This method fixes that.
        /// </summary>
        public void UpdateMonoInfo()
        {
            if (monoBehaviour != null)
            {
                Mono = monoBehaviour;
                EnableOption = enableOption;
            }
        }

        public void UpdateMonoInfo(MonoBehaviour mono)
        {
            if (mono != null)
            {
                Mono = mono;
                EnableOption = enableOption;
            }
        }
        
    }
}