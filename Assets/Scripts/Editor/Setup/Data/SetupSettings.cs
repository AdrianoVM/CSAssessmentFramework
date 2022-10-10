using System;
using UnityEngine;

namespace Setup.Data
{
    [Serializable]
    public class SetupSettings : ScriptableObject
    {
        public bool EnableAccelerationSettings = false;
        public bool ShowAccelerationSettings = false;
    }
}