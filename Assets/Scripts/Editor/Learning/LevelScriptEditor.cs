using System;
using Learning;
using UnityEngine;
using UnityEditor;



    [CustomEditor(typeof(LevelScript))]
    public class LevelScriptEditor : Editor
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

        private void OnSceneGUI()
        {
            var launcher = target as LevelScript;
            if (launcher != null)
            {
                var transform = launcher.transform;
                using (var cc = new EditorGUI.ChangeCheckScope())
                {
                    Vector3 newOffset = transform.InverseTransformPoint(
                        Handles.PositionHandle(
                            transform.TransformPoint(launcher.offset), transform.rotation));
                    if (cc.changed)
                    {
                        Undo.RecordObject(launcher, "Offset Change");
                        launcher.offset = newOffset;
                    }
                }
                using (var cc = new EditorGUI.ChangeCheckScope())
                {
                    var newRadius = Handles.RadiusHandle(
                        transform.rotation, 
                        transform.position, 
                        launcher.damageRadius);
                    if (cc.changed)
                    {
                        Undo.RecordObject(launcher, "Radius Change");
                        launcher.damageRadius = newRadius;
                    }
                }
                
                
            }
            }


    }
