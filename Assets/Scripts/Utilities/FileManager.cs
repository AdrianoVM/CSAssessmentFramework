using System;
using System.IO;
using UnityEngine;

namespace Utilities
{
    //From Unitenow20-Persistent-Data
    public static class FileManager
    {
        public static bool WriteToFile(string fileName, string fileContents, bool isFullPath= false)
        {
            var fullPath = isFullPath? fileName : Path.Combine(Application.dataPath, "Presets",fileName);
            try
            {
                File.WriteAllText(fullPath, fileContents);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to write to {fullPath} with exception {e}");
                return false;
            }
        }

        public static bool LoadFromFile(string fileName, out string result, bool isFullPath = false, bool muteError = false)
        {
            var fullPath = isFullPath? fileName : Path.Combine(Application.dataPath, "Presets",fileName);

            try
            {
                result = File.ReadAllText(fullPath);
                return true;
            }
            catch (Exception e)
            {
                if (!muteError)
                {
                    Debug.LogError($"Failed to read from {fullPath} with exception {e}");
                }
                result = "";
                return false;
            }
        }
    }
}