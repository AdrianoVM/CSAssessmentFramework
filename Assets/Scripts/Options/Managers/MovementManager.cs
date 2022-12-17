using System.Collections.Generic;
using Utilities.Json;

namespace Options.Managers
{
    public class MovementManager : Manager
    {
        //public override string ManagerName { get; set; } = "Movement";

        public override void Load(ref List<InspectorOption> optionsList)
        {

            if (CanSaveOrLoad())
            {
                JsonSaving.LoadInspectorOptions(ref optionsList, nameof(MovementManager), globalInfo.FilePath);
            }
        }
        
        public override void Load()
        {
            Load(ref options);
        }

        public override void LoadUnique(ref List<InspectorOption> optionsList)
        {
            if (CanSaveOrLoad(managerName))
            {
                JsonSaving.LoadInspectorOptions(ref optionsList, nameof(MovementManager), managerPath);
            }
        }

        public override void LoadUnique()
        {
            LoadUnique(ref options);
        }

        public override void Save(ref List<InspectorOption> optionsList)
        {
            if (CanSaveOrLoad())
            {
                JsonSaving.SaveInspectorOptions(ref optionsList, nameof(MovementManager), globalInfo.FilePath);
            }
        }
        
        public override void Save()
        {
            Save(ref options);
        }
        
        public override void SaveUnique(ref List<InspectorOption> optionsList)
        {
            if (CanSaveOrLoad(managerName))
            {
                JsonSaving.SaveInspectorOptions(ref optionsList, nameof(MovementManager), managerPath);
            }
        }
        
        public override void SaveUnique()
        {
            SaveUnique(ref options);
        }
    }
}