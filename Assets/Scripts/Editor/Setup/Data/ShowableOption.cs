using UnityEngine;

namespace Setup.Data
{
    public class ShowableOption
    {
        public MonoBehaviour MonoBehaviour;
        public bool EnableOption = true;
        public bool ExpandOption = true;

        public ShowableOption()
        {
            MonoBehaviour = null;
        }

        public ShowableOption(MonoBehaviour mono)
        {
            MonoBehaviour = mono;
        }

    }
}