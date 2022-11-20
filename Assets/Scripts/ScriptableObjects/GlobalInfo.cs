using UnityEngine;

namespace ScriptableObjects
{
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