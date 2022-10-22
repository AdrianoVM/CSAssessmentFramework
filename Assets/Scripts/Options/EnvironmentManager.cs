namespace Options
{
    public class EnvironmentManager : Manager
    {
        public override void Load()
        {
            JsonSaving.LoadInspectorOptions(options, nameof(EnvironmentManager), "SaveData02.dat");
        }

        public override void Save()
        {
            JsonSaving.SaveInspectorOptions(options, nameof(EnvironmentManager), "SaveData02.dat");
        }
    }
}