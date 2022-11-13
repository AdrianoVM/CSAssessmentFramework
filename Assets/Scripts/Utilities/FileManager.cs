using System;
using System.IO;
using Options;
using Options.Managers;
using UnityEditor;
using UnityEngine;

namespace Utilities
{
    //From Unitenow20-Persistent-Data
    public static class FileManager
    {
        public static bool WriteToFile(string a_FileName, string a_FileContents, bool isFullPath= false)
        {
            //Temporary change
            //var fullPath = Path.Combine(Application.persistentDataPath, a_FileName);

            var fullPath = isFullPath? a_FileName : Path.Combine(Application.dataPath, "Presets",a_FileName);
            try
            {
                File.WriteAllText(fullPath, a_FileContents);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to write to {fullPath} with exception {e}");
                return false;
            }
        }

        public static bool LoadFromFile(string a_FileName, out string result, bool isFullPath = false, bool muteError = false)
        {
            //var fullPath = Path.Combine(Application.persistentDataPath, a_FileName);
            var fullPath = isFullPath? a_FileName : Path.Combine(Application.dataPath, "Presets",a_FileName);

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