using System.Collections.Generic;

[System.Serializable]
public class QuestInstance
{
    public QuestData questDefinition;  // reference to the ScriptableObject
    public List<ObjectiveState> objectives = new List<ObjectiveState>();
    public bool isCompleted = false;

    public QuestInstance(QuestData definition)
    {
        questDefinition = definition;
        foreach (var obj in definition.objectives)
        {
            objectives.Add(new ObjectiveState(obj.requiredAmount));
        }
    }

    public void UpdateProgress(int objectiveIndex, int amount = 1)
    {
        if (objectiveIndex < 0 || objectiveIndex >= objectives.Count) return;

        objectives[objectiveIndex].currentAmount += amount;

        if (AllObjectivesComplete())
        {
            isCompleted = true;
        }
    }

    private bool AllObjectivesComplete()
    {
        foreach (var obj in objectives)
        {
            if (!obj.IsComplete) return false;
        }
        return true;
    }
}

[System.Serializable]
public class ObjectiveState
{
    public int requiredAmount;
    public int currentAmount;

    public bool IsComplete => currentAmount >= requiredAmount;

    public ObjectiveState(int required)
    {
        requiredAmount = required;
        currentAmount = 0;
    }
}

