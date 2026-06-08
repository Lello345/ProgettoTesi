using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [Header("Configurazione Dati")]
    [SerializeField] private NPCProfileSO npcProfile; // Trascina qui lo ScriptableObject (es. Elion)

    [Header("Riferimenti di Sistema")]
    [SerializeField] private OllamaModelHandler modelHandler; // Il gestore della connessione a Ollama
    //[SerializeField] private DialogueView dialogueView;       // La UI gestita con UI Toolkit

    // La memoria dinamica di questa specifica istanza di NPC
    private List<string> _dialogueHistory = new List<string>();

    // Proprietà pubblica per permettere ad altri script (es. il Player) di leggere i dati statici
    public NPCProfileSO Profile => npcProfile;
    
    public void InitInteraction()
    {
        if (npcProfile == null)
        {
            Debug.LogError($"[NPCManager] Manca il profilo NPC su {gameObject.name}");
            return;
        }

        // Resetta la memoria per la nuova conversazione
        _dialogueHistory.Clear();

        // 1. Prendi il JSON del saluto iniziale statico dallo ScriptableObject
        string rawJsonGreeting = npcProfile.GetInitialGreetingJson();

        // 2. Converti il JSON nell'oggetto C# DialogueData
        DialogueData greetingData = JsonUtility.FromJson<DialogueData>(rawJsonGreeting);

        // 3. Salva il saluto nella cronologia dell'NPC per dare contesto alle IA nei turni successivi
        _dialogueHistory.Add($"{npcProfile.npcName}: {greetingData.npc_response}");

        // 4. Passa i dati alla DialogueView per aggiornare la UI con testo e bottoni
        // dialogueView.ShowGreeting(greetingData, this);
    }

    public async Task<DialogueData> SendChoiceAsync(string playerInput)
    {
        // 1. Registra l'input del giocatore nella memoria locale
        _dialogueHistory.Add($"Giocatore: {playerInput}");

        // 2. Sfrutta lo Strategy Pattern: lo ScriptableObject genera il prompt corretto
        string fullPrompt = npcProfile.GeneratePrompt(playerInput, _dialogueHistory);

        // 3. Invia la richiesta a Ollama tramite il Model Handler (asincrono)
        string rawJsonResponse = await modelHandler.SendPromptAsync(fullPrompt);

        // 4. Parsing del JSON ricevuto dal modello locale in un oggetto DialogueData
        DialogueData newDialogueNode = ParseOllamaResponse(rawJsonResponse);

        // 5. Salva la risposta dell'NPC nella cronologia per il prossimo turno
        _dialogueHistory.Add($"{npcProfile.npcName}: {newDialogueNode.npc_response}");

        return newDialogueNode;
    }

    private DialogueData ParseOllamaResponse(string rawResponse)
    {
        try
        {   
            DialogueData data = JsonUtility.FromJson<DialogueData>(rawResponse);
            
            // Sicurezza: se per qualche motivo la lista delle scelte è null (es. Chat Libera)
            data.player_choices ??= new List<string>();

            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[NPCManager] Errore nel parsing del JSON di Ollama: {e.Message}");
            // Fallback per non bloccare il gioco in caso di allucinazione del modello
            return new DialogueData("...Non riesco a concentrarmi, la mia mente è offuscata... (Errore Generazione IA)");
        }
    }
}
