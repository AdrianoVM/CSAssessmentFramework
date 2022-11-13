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
        }
    }
}