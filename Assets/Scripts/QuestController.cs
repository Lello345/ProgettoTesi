using System.Collections.Generic;
using UnityEngine;
using static Quest;

public class QuestController : MonoBehaviour
{
    public static QuestController Instance { get; private set; }
    public List<Quest.QuestProgress> activeQuests;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AcceptQuest(Quest quest)
    {
        if(IsQuestActive(quest.questID)) return;
        activeQuests.Add(new Quest.QuestProgress(quest));
    }

    public bool IsQuestActive(string questID)
    {
        return activeQuests.Exists(q => q.QuestID == questID);
    }
    public bool IsQuestCompleted(string questID)
    {
        QuestProgress quest = activeQuests.Find(q => q.QuestID == questID); 
        return quest != null && quest.IsCompleted;
    }
}
