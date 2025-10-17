using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public List<QuestInstance> activeQuests = new List<QuestInstance>();
    public List<QuestInstance> completedQuests = new List<QuestInstance>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void AddQuest(QuestData questDefinition)
    {
        // Prevent duplicates
        if (activeQuests.Exists(q => q.questDefinition == questDefinition)) return;

        QuestInstance newQuest = new QuestInstance(questDefinition);
        activeQuests.Add(newQuest);
        Debug.Log("Quest started: " + questDefinition.questTitle);
    }

    public void UpdateQuestProgress(string questID, int objectiveIndex, int amount = 1)
    {
        var quest = activeQuests.Find(q => q.questDefinition.questID == questID);
        if (quest != null)
        {
            quest.UpdateProgress(objectiveIndex, amount);

            if (quest.isCompleted)
            {
                activeQuests.Remove(quest);
                completedQuests.Add(quest);
                Debug.Log("Quest completed: " + quest.questDefinition.questTitle);
            }
        }
    }
}