using System;
using Learning;
using UnityEngine;
using UnityEditor;

namespace Options
{
    [CustomEditor(typeof(Option))]
    public class OptionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            LevelScript myLevelScript = (LevelScript) target;

            myLevelScript.experience = EditorGUILayout.IntField("Experience", myLevelScript.experience);
            EditorGUILayout.LabelField("Level", myLevelScript.Level.ToString());
            if (GUILayout.Button("Build Object"))
            {
                myLevelScript.BuildCube();
            }
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
        static void DrawGizmosSelected(LevelScript projectile, GizmoType gizmoType)
        {
            Gizmos.DrawSphere(projectile.transform.position, 0.125f);
        }

    }
}