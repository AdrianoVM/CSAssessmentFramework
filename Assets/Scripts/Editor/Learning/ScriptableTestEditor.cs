using UnityEditor;
using UnityEngine;

namespace Learning
{
    [CustomEditor(typeof(ScriptableTest),true)]
    public class ScriptableTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            ScriptableTest myScriptableTest = (ScriptableTest)target;
            Editor testEditor = Editor.CreateEditor(myScriptableTest.testMono);
            GUILayout.Label(myScriptableTest.testInt.ToString());
            GUILayout.Label(myScriptableTest.testMono.name);
            //testEditor.DrawDefaultInspector();
            //testEditor.OnInspectorGUI();
        }
    }
}