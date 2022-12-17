using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Options.Gameplay;
using UnityEngine;
using SimpleFileBrowser;

namespace DataSaving
{
    /// <summary>
    /// Contains the necessary logic to save data produced during an experiment.
    /// For now only capable of handling headset position and rotation.
    /// Should be expanded to a more general behaviour.
    /// </summary>
    public class DataSaver : MonoBehaviour
    {
        [SerializeField] private float sampleFrequencyInSeconds = 1;
        
        private string _folderName = "";

        public string FolderName
        {
            get => _folderName;
            set
            {
                if (FolderChanged != null) FolderChanged(value);
                _folderName = value;
            }
        }

        public bool SaveData { get; set; }


        /// <summary>
        /// Raised whenever <see cref="FolderName"/> is modified.
        /// </summary>
        public static event Action<string> FolderChanged;
        
        /// <summary>
        /// Raised with <c>true</c> when data is saved on a new file.
        /// Raised with <c>false</c> on game end if no data was saved.
        /// </summary>
        public static event Action<bool> DataSaved; 

        private string _playerPrefDataKey = "DataFolder";
        private float _nextActionTime;
        private List<TransformData> _transformDataList = new();

        /// <summary>
        /// Small class used to contain Position and Rotation vectors
        /// </summary>
        public class TransformData
        {
            public float Time;
            public Vector3 Position;
            public Vector3 Rotation;

            public TransformData(float time, Vector3 position, Vector3 rotation)
            {
                Time = time;
                Position = position;
                Rotation = rotation;
            }
        }

        private void OnEnable()
        {
            GameHandler.GameEnded += WriteData;
        }

        private void Start()
        {
            FileBrowser.SetDefaultFilter(".csv"); // Not sure if necessary
            // Loading folder name from playerPrefs, persistent between sessions.
            if (PlayerPrefs.HasKey(_playerPrefDataKey))
            {
                FolderName = PlayerPrefs.GetString(_playerPrefDataKey);
            }
        }

        private void Update()
        {
            if (GameHandler.State == GameHandler.StateType.Playing)
            {
                var gameTime = Time.time - GameHandler.Instance.StartTime;
                if (gameTime > _nextActionTime ) {
                    _nextActionTime += sampleFrequencyInSeconds;
                    // Adding position and rotation to list
                    Transform cameraTr = GameHandler.Instance.XROrigin.Camera.transform;
                    var data = new TransformData(gameTime, cameraTr.position, cameraTr.rotation.eulerAngles);
                    _transformDataList.Add(data);
                }
            }
        }

        public void SelectDestination()
        {
            if (!FileBrowser.IsOpen)
            {
                StartCoroutine(ShowSaveDialogCoroutine());
            }
        }

        /// <summary>
        /// Displays a save dialog used to pick a location, once it is chosen,
        /// save location in <see cref="FolderName"/> and in playerPrefs
        /// </summary>
        private IEnumerator ShowSaveDialogCoroutine()
        {
            yield return FileBrowser.WaitForSaveDialog( FileBrowser.PickMode.Folders, false, null, null, "Data Folder Location", "Choose Folder" );
            if (FileBrowser.Success && FileBrowser.Result.Length == 1)
            {
                FolderName = FileBrowser.Result[0];
                PlayerPrefs.SetString(_playerPrefDataKey, FolderName);
                PlayerPrefs.Save();
            }
        }
        
        /// <summary>
        /// Writes data to a new csv file with current time as name.
        /// </summary>
        private void WriteData()
        {
            if (SaveData && FolderName != "")
            {
                var fullPath = Path.Combine(FolderName,"CSFData-"+DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")+".csv");
                try
                {
                    if (_transformDataList.Count == 0)
                    {
                        Debug.LogWarning("Nothing to save");
                        return;
                    }
                    
                    var tw = new StreamWriter(fullPath, false);
                    tw.WriteLine("Time, Position_X, Position_Y, Position_Z, Rotation_X, Rotation_Y, Rotation_Z");
                    foreach (TransformData data in _transformDataList)
                    {
                        var newLine = $"{data.Time},{data.Position.x},{data.Position.y},{data.Position.z},{data.Rotation.x},{data.Rotation.y},{data.Rotation.z}";
                        tw.WriteLine(newLine);
                    }
                    tw.Close();
                    
                    if (DataSaved != null) DataSaved(true);
                    Debug.Log("Saved");
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    Debug.LogError("Failed to write data");
                }
            }
            else
            {
                if (DataSaved != null) DataSaved(false);
            }
            _transformDataList.Clear();
        }

        private void OnDisable()
        {
            GameHandler.GameEnded -= WriteData;
        }
    }
}