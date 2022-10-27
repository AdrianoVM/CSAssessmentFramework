using UnityEngine;

namespace Options
{
    public class VisionManager : Manager
    {
        
        public override void Load()
        {
            if (CanSaveOrLoad())
            {
                JsonSaving.LoadInspectorOptions(options, nameof(VisionManager), globalInfo.FilePath);
            }
            
        }
        
        public override void LoadUnique()
        {
            if (CanSaveOrLoad(managerName))
            {
                JsonSaving.LoadInspectorOptions(options, nameof(VisionManager), globalInfo.GetPathOfManager(managerName));
            }
        }

        public override void Save()
        {
            if (CanSaveOrLoad())
            {
                JsonSaving.SaveInspectorOptions(options, nameof(VisionManager), globalInfo.FilePath);
            }
        }
        
        public override void SaveUnique()
        {
            if (CanSaveOrLoad(managerName))
            {
                JsonSaving.SaveInspectorOptions(options, nameof(VisionManager), globalInfo.GetPathOfManager(managerName));
            }
        }
    }
}