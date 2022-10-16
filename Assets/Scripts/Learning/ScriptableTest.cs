using System.Collections.Generic;
using Options;
using UnityEngine;

namespace Learning
{
    [CreateAssetMenu(fileName = "TestScriptableObject", menuName = "ScriptableObjects/testScriptable")]
    public class ScriptableTest : ScriptableObject
    {
        public int testInt;
        public MonoBehaviour testMono;
        public List<ShowableOption> movementOptions;
    }
}