using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class LoadSaveManager : MonoBehaviour
{

    public static LoadSaveManager LS;
    public SaveData activeSave;
    public bool hasDataLoaded;

    private void Awake()
    {
        LS = this;
        LoadData();
    }

    void Start()
    {
        
    }

    public void SaveData()
    {
        string dataPath = Application.persistentDataPath;

        var serializer = new XmlSerializer(typeof(SaveData));
        var stream = new FileStream(dataPath + "/" + activeSave.saveName + ".save", FileMode.Create);
        serializer.Serialize(stream, activeSave);
        stream.Close();

        Debug.Log("Data Saved!");
    }

    public void LoadData()
    {
        string dataPath = Application.persistentDataPath;

        if (System.IO.File.Exists(dataPath + "/" + activeSave.saveName + ".save"))
        {
            var serializer = new XmlSerializer(typeof(SaveData));
            var stream = new FileStream(dataPath + "/" + activeSave.saveName + ".save", FileMode.Open);
            activeSave = serializer.Deserialize(stream) as SaveData;
            stream.Close();

            hasDataLoaded = true;

            Debug.Log("Data Loaded!");

        }
    }

    public void ClearData()
    {
        string dataPath = Application.persistentDataPath;

        if (System.IO.File.Exists(dataPath + "/" + activeSave.saveName + ".save"))
        {
            File.Delete(dataPath + "/" + activeSave.saveName + ".save");

            Debug.Log("Data is clear!");

        }
    }


}

[System.Serializable]
public class SaveData
{
    public string saveName;

    public string[,] toSaveLevelStep;
    public List<LevelData> toSaveLevelDataList = new List<LevelData>();
    public int toSaveCurrentLevel = 1;
    public bool toSaveSwitchSoundStateOn = true;
    public bool toSaveSwitchVibroStateOn = true;
    public bool toSaveIsFirstLaunch = true;
    public bool toSaveCanShowPrivacyScreen = true;
    public bool toSaveIsAlreadyHighlighted = false;
}
