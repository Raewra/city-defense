using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;      
    public GameProgress progress;            
    private string savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            savePath = Path.Combine(Application.persistentDataPath, "save.json");

            // Load existing save or create new one
            progress = Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }



    public void Save()
    {
        // Calculate total stars
        progress.starsTotal = CalculateTotalStars();

        string json = JsonConvert.SerializeObject(progress, Formatting.Indented);
        File.WriteAllText(savePath, json);
        Debug.Log("Game saved at " + savePath);
    }

    public GameProgress Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            Debug.Log("Game loaded from " + savePath);
            return JsonConvert.DeserializeObject<GameProgress>(json);
        }

        Debug.Log("No save file found, creating new progress");
        return new GameProgress();
    }

 

    public void SetDungeonStars(string dungeonName, int stars)
    {
        if (progress.dungeonStars.ContainsKey(dungeonName))
        {
            if (stars > progress.dungeonStars[dungeonName])
            {
                progress.dungeonStars[dungeonName] = stars;
                Save(); // Autosave
            }
        }
        else
        {
            progress.dungeonStars[dungeonName] = stars;
            Save(); // Autosave
        }
    }

    public int GetDungeonStars(string dungeonName)
    {
        if (progress.dungeonStars.TryGetValue(dungeonName, out int stars))
        {
            return stars;
        }
        return 0;
    }

    private int CalculateTotalStars()
    {
        int total = 0;
        foreach (var kvp in progress.dungeonStars)
        {
            total += kvp.Value;
        }
        return total;
    }

   

    public void AddGold(int amount)
    {
        progress.gold += amount;
        Save(); // Autosave
    }

    public void AddSupplies(int amount)
    {
        progress.supplies += amount;
        Save(); // Autosave
    }

    public void SetPlayerLevel(int level)
    {
        progress.playerLevel = level;
        Save(); // Autosave
    }

    public int GetGold() => progress.gold;
    public int GetSupplies() => progress.supplies;
    public int GetPlayerLevel() => progress.playerLevel;
    public int GetTotalStars() => progress.starsTotal;

   
}
