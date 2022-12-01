using System;
using Options.Environment;
using UnityEditor;
using UnityEngine;

namespace Options
{
    [CustomEditor(typeof(TerrainModifier))]
    public class TerrainModifierEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var myTerrainModifier = target as TerrainModifier;
            
            base.OnInspectorGUI();

            if (GUILayout.Button("Apply"))
            {
                if (myTerrainModifier != null)
                {
                    Undo.RegisterCompleteObjectUndo(myTerrainModifier.TargetTerrain.terrainData, "Apply terrain Changes");
                    if (myTerrainModifier != null) myTerrainModifier.Apply();
                }
            }
        }
    }
}