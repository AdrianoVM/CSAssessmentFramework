﻿namespace Options
{
    public class MovementManager : Manager
    {
        public override string ManagerName { get; set; } = "Movement";

        public override void Load()
        {

            JsonSaving.LoadInspectorOptions(options, nameof(MovementManager), "SaveData03.dat");
        }

        public override void Save()
        {
            JsonSaving.SaveInspectorOptions(options, nameof(MovementManager), "SaveData03.dat");
        }
    }
}