using System.Collections.Generic;
using Utilities.Json;

namespace Options.Managers
{
    public class ExperimentManager : Manager
    {

        public override void Load(ref List<InspectorOption> optionsList)
        {

            if (CanSaveOrLoad())
            {
                JsonSaving.LoadInspectorOptions(ref optionsList, nameof(ExperimentManager), globalInfo.FilePath);
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
                JsonSaving.LoadInspectorOptions(ref optionsList, nameof(ExperimentManager), managerPath);
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
                JsonSaving.SaveInspectorOptions(ref optionsList, nameof(ExperimentManager), globalInfo.FilePath);
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
                JsonSaving.SaveInspectorOptions(ref optionsList, nameof(ExperimentManager), managerPath);
            }
        }
        
        public override void SaveUnique()
        {
            SaveUnique(ref options);
        }
    }
}