using UnityEngine;

[CreateAssetMenu(fileName = "NPCDialogueSO", menuName = "NPC Dialogue")]
public class NPCDialogueSO : ScriptableObject 
{
    public string npcName;
    public Sprite npcPortrait;
    public string[] dialogueLines;
    public bool[] enrich;
}

