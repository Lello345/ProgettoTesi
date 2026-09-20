using UnityEngine;

[CreateAssetMenu(fileName = "NPCDialogueSO", menuName = "NPC Dialogue")]
public class NPCDialogueSO : ScriptableObject 
{
    public string npcName;
    public Sprite npcPortrait;
    public string[] dialogueLines;
    public bool[] endDialogueLines;
    public bool[] enrich;
    //public string[] addElicitedResponses; 
    public DialogueChoice[] dialogueChoices;
}

[System.Serializable]
public class DialogueChoice
{
    public int dialogueIndex;
    public string[] choices;
    public int[] nextDialogueIndex;
}

