using System;
using System.IO;
using Options;
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


        public static void FileButtons(GlobalInfo info, Manager manager, ref bool show)
        {
            FileButtonsInternal(info, new[]{manager}, true, ref show);
        }

        public static void FileButtons(GlobalInfo info, Manager[] managers, ref bool show)
        {
            FileButtonsInternal(info, managers, false, ref show);
        }
        private static void FileButtonsInternal(GlobalInfo info, Manager[] managers, bool isUnique, ref bool showButtons)
        {
            showButtons = EditorGUILayout.Foldout(showButtons, "Save System", true);
            if (!showButtons)
            {
                return;
            }
            var fileName = isUnique ? info.GetPathOfManager(managers[0].managerName, true) : info.FilePath;
            GUILayout.Label("Chosen File: "+Path.GetFileName(fileName), EditorStyles.largeLabel);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Load From", GUILayout.MaxWidth(100)))
            {
                var path = EditorUtility.OpenFilePanel("Where to Load From", "", "dat");
                if (path.Length != 0)
                {
                    if (info != null)
                    {
                        if (isUnique)
                        {
                            managers.ForEach(m => info.AddToManagerFilePaths(m.managerName, path));
                            managers.ForEach(m => m.LoadUnique());
                        }
                        else
                        {
                            info.FilePath = path;
                            managers.ForEach(m => m.Load());
                        }
                        
                    }
                    else
                    {
                        Debug.LogWarning("No Scriptable Object to save into");
                    }
                    
                }
            }

            if (GUILayout.Button("Save As", GUILayout.MaxWidth(100)))
            {
                var path = EditorUtility.SaveFilePanel("Where to Save", "", "OptionPreset", "dat");
                if (path.Length != 0)
                {
                    if (info != null)
                    {
                        if (isUnique)
                        {
                            managers.ForEach(m => info.AddToManagerFilePaths(m.managerName, path));
                            managers.ForEach(m => m.SaveUnique());
                        }
                        else
                        {
                            info.FilePath = path;
                            managers.ForEach(m => m.Save());
                        }
                        
                    }
                    else
                    {
                        Debug.LogWarning("No Scriptable Object to save into");
                    }
                    
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Load",GUILayout.MaxWidth(100)))
            {
                if (isUnique)
                {
                    managers.ForEach(m => m.LoadUnique());
                }
                else
                {
                    managers.ForEach(m => m.Load());
                }
                
            }
            if (GUILayout.Button("Save",GUILayout.MaxWidth(100)))
            {
                if (isUnique)
                {
                    managers.ForEach(m => m.SaveUnique());
                }
                else
                {
                    managers.ForEach(m => m.Save());
                }
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
    }
}