using System;using UnityEngine;

public enum SaveFileType {
    PersistentData,
    Slot
}

public static class SaveManager
{
    public static SaveData CurrentSlotSaveData { get; private set; }
    public static SaveData SaveSlot { get; private set; }
    public static SaveData PersistentSaveData { get; private set; }

    static SaveManager()
    {
        CurrentSlotSaveData = new SaveData();
        PersistentSaveData = new SaveData(false);
        SaveSlot = new SaveData();
    }

    public static void LoadFile(SaveFileType saveType)
    {
        if (saveType == SaveFileType.PersistentData)
        {
#if UNITY_EDITOR
            PersistentSaveData.LoadFile("editor_persistent_data");
#else
            PersistentSaveData.LoadFile("persistent_data");
#endif
        }
        else if (saveType == SaveFileType.Slot)
        {
            SaveSlot.LoadFile("SaveSlot");
        }
    }

    public static void LoadFiles()
    {
        LoadFile(SaveFileType.PersistentData);
        LoadFile(SaveFileType.Slot);
    }

    public static void SaveFile(SaveData data)
    {
        data.SaveFile();
    }

    public static void DebugData()
    {
        DebugDataForSave(PersistentSaveData);
        DebugDataForSave(CurrentSlotSaveData);
    }

    private static void DebugDataForSave(SaveData data)
    {
        foreach (var entry in data.Data)
        {
            Debug.Log($"{entry.key} is {entry.GetValue()}");
        }
    }
}