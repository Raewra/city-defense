using UnityEngine;
using System.IO;
using Newtonsoft.Json; 

public static class SaveSystem
{
    private static string path = Application.persistentDataPath + "/save.json";

    public static void Save(GameProgress progress)
    {
        // Serialize using Newtonsoft
        string json = JsonConvert.SerializeObject(progress, Formatting.Indented);
        File.WriteAllText(path, json);
        Debug.Log("Game saved at " + path);
    }

    public static GameProgress Load()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<GameProgress>(json);
        }

        Debug.Log("No save file found, creating new progress");
        return new GameProgress(); // return empty if no save yet
    }
}
