using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[Serializable]
public class SaveData
{
    private string savePath = Application.persistentDataPath;

    public List<SerializableValue> Data = new List<SerializableValue>();
    private string filename;
    public bool isSlot;

    public SaveData(bool isSlot = true)
    {
        this.isSlot = isSlot;
        if (isSlot) SetDefaultSlotData();
        else SetDefaultPersistentData();
    }

    public void LoadFile(string file)
    {
        filename = file;
        string fullPath = Path.Combine(savePath, file + ".json");

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = File.ReadAllText(fullPath);

                SaveData loadData = JsonUtility.FromJson<SaveData>(dataToLoad);
                Data = loadData.Data;

                Debug.Log($"Saved Data {file} succesfully loaded");
            }
            catch (Exception e)
            {
                Debug.LogError("Error occurred when trying to load data from file: " + fullPath + "\n" + e);
            }
        }
        else
        {
            Debug.Log($"There is no {file} file, setting default data");
            SaveManager.SaveFile(this);
        }
    }

    public bool DoesSlotExist()
    {
        string fullPath = Path.Combine(savePath, "SaveSlot.json");

        if (File.Exists(fullPath)) return true;
        else return false; }

    public void SaveFile()
    {
        string fullPath = Path.Combine(savePath, filename + ".json");

        try
        {
            string dataToStore = JsonUtility.ToJson(this, true);
            Debug.Log(fullPath);
            File.WriteAllText(fullPath, dataToStore);
            Debug.Log($"Data {filename + ".json"} succesfully saved");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error occurred when trying to save data {filename + ".json"} to file: " + fullPath + "\n" + e);
        }
    }
    /// <summary>
    /// Sets a specific saved data to a value, possible values are: string, bool, int, float and Vector2
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>

    public void SetData(string key, object value)
    {
        var kv = Data.Find(x=> x.key == key);
        if (kv == null)
        {
            Data.Add(new SerializableValue(key, value));
        }
        else
        {
            kv.SetValue(value);
        }
    }

    public T GetData<T>(string key)
    {
        var kv = Data.Find(x => x.key == key);
        if (kv == null)
        {
            Debug.LogError($"Can't load data of type {typeof(T)} at key {key}");
            return default(T);
        }
        else
        {
            return (T)kv.GetValue();
        }
    }

    public void SetDefaultSlotData()
    {
        Data.Clear();
        SetData("hasPlayed", false);
    }

    private void SetDefaultPersistentData()
    {
        SetData("settings.language", "en");

        SetData("settings.maxVolume", 1f);
        SetData("settings.maxMusicVolume", 1f);
        SetData("settings.maxSfxVolume", 1f);

        SetData("settings.maxFps", 60);

        List<Resolution> resList = Screen.resolutions.ToList();
        Resolution res = resList.Find(x => x.width == Screen.currentResolution.width && x.height == Screen.currentResolution.height);
        SetData("settings.resolution", resList.IndexOf(res));
        SetData("settings.fullscreen", true);
        SetData("settings.audio_offset", 0f);

        SetData("lastPlayedSlot", -1);

        // Enhancements by default
        SetData("enhancement.statHPup.unlocked", true);
        SetData("enhancement.statATKup.unlocked", true);
        SetData("enhancement.statDEFup.unlocked", true);
        SetData("enhancement.statPENup.unlocked", true);
        SetData("enhancement.statCRITCHANCEup.unlocked", true);
        SetData("enhancement.statCRITDMGup.unlocked", true);
        SetData("enhancement.statEXPRANGEup.unlocked", true);
        SetData("enhancement.statEXPMULTIup.unlocked", true);
        SetData("enhancement.statHASTEup.unlocked", true);
        SetData("enhancement.statREROLLSup.unlocked", true);
        SetData("enhancement.statMOVRANGEup.unlocked", true);
    }
}