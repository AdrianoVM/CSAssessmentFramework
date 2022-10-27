namespace Options
{
    public class EnvironmentManager : Manager
    {
        public override void Load()
        {

            if (CanSaveOrLoad())
            {
                JsonSaving.LoadInspectorOptions(options, nameof(EnvironmentManager), globalInfo.FilePath);
            }
        }
        
        
        public override void LoadUnique()
        {
            if (CanSaveOrLoad(managerName))
            {
                JsonSaving.LoadInspectorOptions(options, nameof(EnvironmentManager), globalInfo.GetPathOfManager(managerName));
            }
        }

        public override void Save()
        {
            if (CanSaveOrLoad())
            {
                JsonSaving.SaveInspectorOptions(options, nameof(EnvironmentManager), globalInfo.FilePath);
            }
        }
        
        public override void SaveUnique()
        {
            if (CanSaveOrLoad(managerName))
            {
                JsonSaving.SaveInspectorOptions(options, nameof(EnvironmentManager), globalInfo.GetPathOfManager(managerName));
            }
        }
    }
}