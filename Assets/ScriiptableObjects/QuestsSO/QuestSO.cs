using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Quests/Quest")]
public class Quest : ScriptableObject
{
    public string questID;
    public string questName;
    public string description;
    public bool IsCompleted = false;
    //public List<QuestObjective> objectives;

    private void OnEnable()
    {
        IsCompleted = false;
    }
    private void OnValidate()
    {
        if(string.IsNullOrEmpty(questID))
        {
            questID = questName + Guid.NewGuid().ToString();
        }
    }

    /*
    [System.Serializable]
    public class QuestObjective
    {
        public string objectiveID;
        public string description;
        public int requiredAmount;
        public int currentAmount;
        public bool IsCompleted => currentAmount >= requiredAmount;
    }

    [System.Serializable]
    public class QuestProgress
    {
        public Quest quest;
        public List<QuestObjective> objectives;
        public QuestProgress(Quest quest)
        {
            this.quest = quest;
            objectives = new List<QuestObjective>();

            //Deep copy
            foreach (var objective in quest.objectives)
            {
                QuestObjective newObjective = new QuestObjective
                {
                    objectiveID = objective.objectiveID,
                    description = objective.description,
                    requiredAmount = objective.requiredAmount,
                    currentAmount = 0
                };
                objectives.Add(newObjective);
            }
        }

        public bool IsCompleted => objectives.TrueForAll(obj => obj.IsCompleted);
        public string QuestID => quest.questID;
    }
    */
}
