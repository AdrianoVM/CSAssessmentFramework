using UnityEngine;

namespace Options
{
    public class VisionManager : Manager
    {
        
        public override void Load()
        {
            JsonSaving.LoadInspectorOptions(options, nameof(VisionManager), fileName);
        }

        public override void Save()
        {
            JsonSaving.SaveInspectorOptions(options, nameof(VisionManager), fileName);
        }
    }
}