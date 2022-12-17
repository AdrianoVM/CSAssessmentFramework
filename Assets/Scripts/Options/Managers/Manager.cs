using System.Collections.Generic;
using System.Linq;
using ScriptableObjects;
using UnityEngine;

namespace Options.Managers
{
    public abstract class Manager : MonoBehaviour
    {

        [SerializeReference]
        public List<InspectorOption> options = new(Enumerable.Repeat(new InspectorOption(), 1));
        public GlobalInfo globalInfo;
        
        public delegate void ManagerDelegate(ref List<InspectorOption> optionsList);

        public string managerName = "Manager";

        public string managerPath = "";

        public bool showFileButtons;


        public abstract void Load();
        
        public abstract void Load(ref List<InspectorOption> optionsList);

        public abstract void LoadUnique();
        
        public abstract void LoadUnique(ref List<InspectorOption> optionsList);


        public abstract void Save();

        public abstract void Save(ref List<InspectorOption> optionsList);

        public abstract void SaveUnique();

        public abstract void SaveUnique(ref List<InspectorOption> optionsList);

        protected bool CanSaveOrLoad()
        {
            if (globalInfo == null || globalInfo.FilePath == "")
            {
                Debug.LogError(nameof(globalInfo)+" not set up");
                return false;
            }

            return true;
        }
        
        protected bool CanSaveOrLoad(string savedName)
        {
            if (managerPath != "")
            {
                return true;
            }
            else
            {
                Debug.LogError("Path not set up for "+savedName);
                return false;
            }
        }
    }
}