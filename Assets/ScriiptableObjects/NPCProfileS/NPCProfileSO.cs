using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NuovoProfiloNPC", menuName = "AI Dialogue/Profilo NPC")]
public class NPCProfileSO : ScriptableObject
{
    [Header("Dati dell'NPC")]
    public string npcName;
    [Header("Personalità e Conoscenza (Prompt di Sistema)")]
    [Tooltip("Qui inserisci il background, i segreti e le regole di linguaggio dell'NPC.")] 
    [TextArea(5, 10)] public string npcStory; // Qui scrivi la storia/personalità dell'NPC
    
    [Header("Saluto Iniziale Personale (Già in JSON)")]
    [Tooltip("Il JSON preconfezionato da mostrare all'inizio per evitare la latenza di Ollama.")]
    [TextArea(3, 6)] public string initialGreetingJson; 
    // Esempio da inserire nell'inspector: 
    // {"npc_response": "Salute viandante!", "player_choices": ["Chi sei?", "Cosa vendi?", "Arrivederci"]}

    [Header("Strategia di dialogo")]
    [SerializeField] DialogueStrategySO dialogueStrategy;

    public string GetInitialGreetingJson()
    {
        return initialGreetingJson;
    }

    public string GeneratePrompt(string playerInput, List<string> conversationHistory)
    {
        if (dialogueStrategy != null)
        {
            return dialogueStrategy.FormatPrompt(npcStory, playerInput, conversationHistory);
        }
        else
        {
            Debug.LogError($"[NPCProfileSO] Manca la Dialogue Strategy sul profilo di '{npcName}'! Impossibile formattare il prompt.");
            // Ritorna un prompt minimo per non far crashare il sistema
            return $"{npcStory}\n\nUser: {playerInput}";
        }
    }
}
