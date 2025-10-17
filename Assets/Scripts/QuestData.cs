using UnityEngine;


[CreateAssetMenu(fileName = "NewQuest", menuName = "Quests/Quest")]
public class QuestData : ScriptableObject
{
    public string questID;
    public string questTitle;
    [TextArea] public string questDescription;

    public ObjectiveData[] objectives;
    // public Reward[] rewards;
}

[System.Serializable]
public class ObjectiveData
{
    public string description;
    public int requiredAmount;
}
