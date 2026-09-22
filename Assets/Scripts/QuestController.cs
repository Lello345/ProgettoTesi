using System.Collections.Generic;
using UnityEngine;
using static Quest;

public class QuestController : MonoBehaviour
{
    public static QuestController Instance { get; private set; }
    //public List<Quest.QuestProgress> activeQuests;
    public List<Quest> activeQuests;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AcceptQuest(Quest quest)
    {
        if(IsQuestActive(quest.questID)) return;
        activeQuests.Add(quest);
    }

    public bool IsQuestActive(string questID)
    {
        //return activeQuests.Exists(q => q.QuestID == questID);
        return activeQuests.Exists(q => q.questID == questID);
    }
    public void CompleteQuest(string questID)
    {
        Quest quest = activeQuests.Find(q => q.questID == questID);
        if (quest != null)
        {
            quest.IsCompleted = true;
        }
    }
    public bool IsQuestCompleted(string questID)
    {
        //QuestProgress quest = activeQuests.Find(q => q.QuestID == questID); 
        Quest quest = activeQuests.Find(q => q.questID == questID);
        return quest != null && quest.IsCompleted;
    }
}
