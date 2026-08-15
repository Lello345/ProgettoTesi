using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class NPCManager : MonoBehaviour, IInteractable
{
    [Header("Configurazione Dati")]
    [SerializeField] private NPCProfileSO npcProfile; // Trascina qui lo ScriptableObject (es. Elion)

    [Header("Riferimenti di Sistema")]
    [SerializeField] private OllamaModelHandler modelHandler; // Il gestore della connessione a Ollama
    [SerializeField] private DialogueView dialogueView;       // La UI gestita con UI Toolkit

    // La memoria dinamica di questa specifica istanza di NPC
    private List<string> _dialogueHistory = new List<string>();
    private DialogueData _currentDialogueNode; // Memorizza l'ultimo nodo di dialogo ricevuto dall'IA

    // Proprietà pubblica per permettere ad altri script (es. il Player) di leggere i dati statici
    public NPCProfileSO Profile => npcProfile;

    private void OnEnable()
    {
        DialogueView.OnPlayerInputSubmittedEvent += HandlePlayerInput;
    }

    private async void HandlePlayerInput(string obj)
    {
        DialogueData nextNode = await SendChoiceAsync(obj);
        dialogueView.ShowDialogue(nextNode);
    }

    private void OnDisable()
    {
        DialogueView.OnPlayerInputSubmittedEvent -= HandlePlayerInput;
    }


    public void Interact()
    {
        if (_dialogueHistory.Count == 0)
        {
            InitInteraction();
        }
        else
        {
            dialogueView.ShowDialogue(_currentDialogueNode);
        }
    }
    public bool CanInteract()
    {
        if (dialogueView.wrapper.style.display == DisplayStyle.Flex)
        {
            return false; // Se la UI è già aperta, non permettere ulteriori interazioni
        }else
        {
            return true; // Altrimenti, permetti l'interazione
        }
    }

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
        dialogueView.ShowDialogue(greetingData);
        _currentDialogueNode = greetingData; // Aggiorna il nodo corrente
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

        _currentDialogueNode = newDialogueNode;
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
