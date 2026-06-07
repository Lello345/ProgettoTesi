using UnityEngine;

[CreateAssetMenu(fileName = "NuovoProfiloNPC", menuName = "AI Dialogue/Profilo NPC")]
public class NPCProfileSO : ScriptableObject
{
    [Header("Dati dell'NPC")]
    public string npcName;
    [TextArea(5, 10)] public string npcStory; // Qui scrivi la storia/personalità dell'NPC
    
    [Header("Saluto Iniziale Personale (Già in JSON)")]
    [TextArea(3, 6)] public string initialGreetingJson; 
    // Esempio da inserire nell'inspector: 
    // {"npc_response": "Salute viandante!", "player_choices": ["Chi sei?", "Cosa vendi?", "Arrivederci"]}

    [Header("Strategia di dialogo")]
    [SerializeReference] IDialogueStrategy dialogueStrategy;
}
