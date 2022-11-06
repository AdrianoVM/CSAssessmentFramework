using UnityEditor;
using UnityEngine;
using Utilities;

namespace ScriptableObjects
{
    [CustomEditor(typeof(GlobalInfo),true)]
    public class GlobalInfoEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var myGlobalInfo = (GlobalInfo)target;

            GUILayout.Label("Main Path");
            GUILayout.Label(myGlobalInfo.FilePath);
            GUILayout.Space(2);
            GUILayout.Label("Manager Editors");
            foreach (var (managerName, path) in myGlobalInfo.ManagerFilePaths)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(managerName );
                GUILayout.Label(path);
                GUILayout.EndHorizontal();
            }
        }
    }
}