using System.IO;
using Data;
using UnityEngine;

public static class SaveManager
{
    private static readonly string _savePath = Path.Combine(Application.persistentDataPath, "savegame.json");

    public static void Save(SaveData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(_savePath, json);
            Debug.Log($"[SaveManager] Game saved → {_savePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveManager] Save failed: {e.Message}");
        }
    }

    public static SaveData Load()
    {
        try
        {
            if (!File.Exists(_savePath))
            {
                Debug.Log("[SaveManager] No save file found, starting fresh");
                return null;
            }

            string json = File.ReadAllText(_savePath);
            var data   = JsonUtility.FromJson<SaveData>(json);
            Debug.Log($"[SaveManager] Save loaded → {data.columns}x{data.rows} seed:{data.seed}");
            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveManager] Load failed: {e.Message}");
            return null;
        }
    }

    public static bool HasSave() => File.Exists(_savePath);

    public static void DeleteSave()
    {
        if (!File.Exists(_savePath)) return;
        File.Delete(_savePath);
        Debug.Log("[SaveManager] Save file deleted");
    }
}