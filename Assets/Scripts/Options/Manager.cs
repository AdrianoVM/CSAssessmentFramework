using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Options
{
    public abstract class Manager : MonoBehaviour
    {

        public List<InspectorOption> options = new(Enumerable.Repeat(new InspectorOption(), 1));
        public string fileName = "saveData.dat";

        [field: SerializeField]
        public virtual string ManagerName { get; set; } = "Manager";


        public abstract void Load();


        public abstract void Save();
    }
}