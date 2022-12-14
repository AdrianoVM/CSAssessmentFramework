using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Options.Gameplay;
using UnityEngine;
using SimpleFileBrowser;
using TMPro;

namespace DataSaving
{
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


        public static event Action<string> FolderChanged;
        public static event Action<bool> DataSaved; 

        private string _playerPrefDataKey = "DataFolder";
        private float _nextActionTime;
        private List<TransformData> _transformDataList = new();

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
            FileBrowser.SetDefaultFilter(".csv");
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
                StartCoroutine(ShowLoadDialogCoroutine());
            }
        }

        private IEnumerator ShowLoadDialogCoroutine(string initFilename=null)
        {
            yield return FileBrowser.WaitForSaveDialog( FileBrowser.PickMode.Folders, false, null, initFilename, "Data Folder Location", "Choose Folder" );
            if (FileBrowser.Success && FileBrowser.Result.Length == 1)
            {
                FolderName = FileBrowser.Result[0];
                PlayerPrefs.SetString(_playerPrefDataKey, FolderName);
                PlayerPrefs.Save();
            }
        }
        

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
                    var csv = new StringBuilder();
                    var tw = new StreamWriter(fullPath, false);
                    csv.AppendLine("Time, Position_X, Position_Y, Position_Z, Rotation_X, Rotation_Y, Rotation_Z");
                    tw.WriteLine("Time, Position_X, Position_Y, Position_Z, Rotation_X, Rotation_Y, Rotation_Z");
                    foreach (TransformData data in _transformDataList)
                    {
                        
                        var newLine = $"{data.Time},{data.Position.x},{data.Position.y},{data.Position.z},{data.Rotation.x},{data.Rotation.y},{data.Rotation.z}";
                        csv.AppendLine(newLine);
                        tw.WriteLine(newLine);
                    }
                    tw.Close();
                    //FileBrowserHelpers.AppendTextToFile(fullPath,csv.ToString());
                    
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