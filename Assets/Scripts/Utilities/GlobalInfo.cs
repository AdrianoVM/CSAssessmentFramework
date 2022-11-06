using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Options;
using UnityEngine;

namespace Utilities
{
    [CreateAssetMenu(fileName = "GlobalInfo", menuName = "ScriptableObjects/GlobalInfo", order = 0)]
    public class GlobalInfo : ScriptableObject
    {
        private string _filePath = "";

        public string FilePath
        {
            get => _filePath;
            set => _filePath = value;
        }

        private Dictionary<string, string> _managerFilePaths = new();

        public Dictionary<string, string> ManagerFilePaths => _managerFilePaths;

        public void AddToManagerFilePaths(string managerName, string path)
        {
            _managerFilePaths[managerName] = path;
        }

        public string GetPathOfManager(string managerName, bool stopWarning=false)
        {
            try
            {
                return _managerFilePaths[managerName];
            }
            catch (Exception e)
            {
                if (!stopWarning)
                {
                    Debug.LogError(managerName+" has no associated Path");
                }
                
                return "";
            }
            
        }

        public void CleanManagerFilePaths(string[] managerNames)
        {
            _managerFilePaths = _managerFilePaths
                .Where(kvp => managerNames.Contains(kvp.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}