using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

namespace Options
{
    public abstract class Manager : MonoBehaviour
    {

        public List<InspectorOption> options = new(Enumerable.Repeat(new InspectorOption(), 1));
        public GlobalInfo globalInfo;

        public string managerName = "Manager";


        public abstract void Load();

        public abstract void LoadUnique();


        public abstract void Save();

        public abstract void SaveUnique();


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
            if (globalInfo == null || globalInfo.GetPathOfManager(savedName) == "")
            {
                Debug.LogError(nameof(globalInfo)+" not set up for "+savedName);
                return false;
            }

            return true;
        }
    }
}