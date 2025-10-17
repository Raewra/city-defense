using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameProgress
{
    public Dictionary<string, int> dungeonStars = new Dictionary<string, int>();
    public int playerLevel;
    public int gold;
    public int supplies;
    public int starsTotal;

   
    public List<string> hiredMercenaries = new List<string>();
  
}
