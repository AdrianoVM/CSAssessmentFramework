namespace Options
{
    public class VisionManager : Manager
    {
        public override void Load()
        {
            JsonSaving.LoadInspectorOptions(options, nameof(VisionManager), "SaveData03.dat");
        }

        public override void Save()
        {
            JsonSaving.SaveInspectorOptions(options, nameof(VisionManager), "SaveData03.dat");
        }
    }
}