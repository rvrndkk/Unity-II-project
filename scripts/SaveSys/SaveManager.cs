using UnityEngine;
using System.IO;

public static class SaveManager
{
    private static string savePath = Path.Combine(Application.persistentDataPath, "savegame.json");

    public static void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"<color=green>Игра сохранена в: {savePath}</color>");
    }

    public static SaveData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<SaveData>(json);
        }
        
        Debug.LogWarning("Файл сохранения не найден. Создаем новый профиль.");
        return new SaveData();
    }
    
    public static void DeleteSave()
    {
        if (File.Exists(savePath)) File.Delete(savePath);
    }
}