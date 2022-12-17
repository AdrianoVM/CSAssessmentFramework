using UnityEngine;

namespace ScriptableObjects
{
    /// <summary>
    /// <see cref="ScriptableObject"/> containing information that could be useful for all components and editors.
    /// </summary>
    [CreateAssetMenu(fileName = "GlobalInfo", menuName = "ScriptableObjects/GlobalInfo", order = 0)]
    public class GlobalInfo : ScriptableObject
    {
        private string _filePath = "";

        public string FilePath
        {
            get => _filePath;
            set => _filePath = value;
        }
    }
}