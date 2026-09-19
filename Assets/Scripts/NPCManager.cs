using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class NPCManager : MonoBehaviour, IInteractable
{
    [Header("Configurazione Dati")]
    

    [Header("Riferimenti di Sistema")]
    [SerializeField] private OllamaModelHandler modelHandler; // Il gestore della connessione a Ollama



    public void Interact()
    {
        
    }
    public bool CanInteract()
    {
        return true;
    }

    public void InitInteraction()
    {
        
    }

    /*
    public async Task<DialogueData> SendChoiceAsync(string playerInput){}
    */
    
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
