namespace Options
{
    public class MovementManager : Manager
    {
        //public override string ManagerName { get; set; } = "Movement";

        public override void Load()
        {

            if (CanSaveOrLoad())
            {
                JsonSaving.LoadInspectorOptions(options, nameof(MovementManager), globalInfo.FilePath);
            }
        }

        public override void LoadUnique()
        {
            if (CanSaveOrLoad(managerName))
            {
                JsonSaving.LoadInspectorOptions(options, nameof(MovementManager), globalInfo.GetPathOfManager(managerName));
            }
        }

        public override void Save()
        {
            if (CanSaveOrLoad())
            {
                JsonSaving.SaveInspectorOptions(options, nameof(MovementManager), globalInfo.FilePath);
            }
        }
        
        public override void SaveUnique()
        {
            if (CanSaveOrLoad(managerName))
            {
                JsonSaving.SaveInspectorOptions(options, nameof(MovementManager), globalInfo.GetPathOfManager(managerName));
            }
        }
    }
}