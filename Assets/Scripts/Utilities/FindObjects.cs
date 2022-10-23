using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities
{
    public static class FindObjects
    {
        /// <summary>
        /// Finds a MonoBehaviour in the active scene by matching type, and if multiple, by name
        /// </summary>
        /// <param name="typeToken"></param>
        /// <param name="nameToken"></param>
        /// <returns>The found MonoBehaviour or null if not found</returns>
        public static Component FindInScene(Type typeToken, string nameToken)
        {
            var foundObjects = Object.FindObjectsOfType(typeToken, true);
            // TODO: add condition if there is only one, disregard name
            if (foundObjects.Length == 1)
            {
                if (foundObjects.First().name != nameToken)
                {
                    Debug.Log("Name is not the same but type is: "+ nameToken);
                }
                return (Component)foundObjects.First();
            }
            foreach (Object foundObject in foundObjects)
            {
                if (foundObject.name == nameToken)
                {
                    return (Component)foundObject;
                }
            }
            Debug.Log("Not found: "+nameToken);
            return null;
        }

        public static Object FindInAssets(string guid, Type type)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (path == "")
            {
                //TODO: raise event
                Debug.Log("Object of type "+ type.Name+" not found in assets");
                return null;
            }
            return AssetDatabase.LoadAssetAtPath(path,type);
        }
    }
}