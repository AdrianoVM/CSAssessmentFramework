using UnityEditor;
using UnityEngine;

namespace Learning
{
    public class SceneDirtyFlagChecker : MonoBehaviour 
    { 
   
#if UNITY_EDITOR
        static SceneDirtyFlagChecker() 
        { 
            Undo.postprocessModifications += OnPostProcessModifications; 
        }
 
        private static UndoPropertyModification[] OnPostProcessModifications(UndoPropertyModification[] propertyModifications) 
        { 
            Debug.LogWarning($"Scene was marked Dirty by number of objects = {propertyModifications.Length}"); 
            for (int i = 0; i < propertyModifications.Length; i++) 
            { 
                Debug.LogWarning($"currentValue.target = {propertyModifications[i].currentValue.target}", propertyModifications[i].currentValue.target); 
            } 
            return propertyModifications; 
        } 
#endif
    } 
}