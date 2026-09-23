using UnityEngine;

[CreateAssetMenu(fileName = "NPCDialogueSO", menuName = "NPC Dialogue")]
public class NPCDialogueSO : ScriptableObject 
{
    public string npcName;
    public Sprite npcPortrait;
    public string[] dialogueLines;
    public bool[] endDialogueLines;
    public Enrichment[] enrich;
    public DialogueChoice[] dialogueChoices;
    public int questInProgressIndex;
    public int questCompletedIndex;
    public Quest quest;
}

[System.Serializable]
public class QuestCompletion
{
    public int choiceIndexThatCompletesQuest = -1;
    public int dialogueIndexThatCompletesQuest = -1;
    public Quest questToComplete;
}

[System.Serializable]
public class Enrichment{
    public int dialogueIndex;
    public string[] addElicitedResponses;
}
[System.Serializable]
public class DialogueChoice
{
    public int dialogueIndex;
    public string[] choices;
    public int[] nextDialogueIndex;
    public bool[] givesQuest;
    public QuestCompletion questCompletion;
}



