using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class NPCManager : MonoBehaviour, IInteractable
{
    [Header("Configurazione Dati")]
    private DialogueController dialogueController;
    public NPCDialogueSO dialogueData;
    public NPCBackgroundSO backgroundData;
    private int dialogueIndex;
    private bool isDialogueActive;
    
    [Header("Riferimenti di Sistema")]
    [SerializeField] private OllamaModelHandler modelHandler; // Il gestore della connessione a Ollama

    void Start()
    {
        dialogueController = DialogueController.Instance;
    }

    public bool CanInteract()
    {
        Debug.Log("can interact");
        return true;
    }
    public void Interact()
    {
        Debug.Log("interact");
        if(dialogueData == null)
        {
            return;
        }

        if (!isDialogueActive)
        {
            StartDialogue();
        }
        else
        {
            NextLine();
        }
    }
    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;

        dialogueController.SetNpcInfo(dialogueData.npcName, dialogueData.npcPortrait);

        dialogueController.ShowDialogueUI(true);
        //TODO: Stoppa il movimento

        TypeLine();
    }
    void NextLine()
    {
        dialogueController.ClearChoices();
        //Check end dialogue lines
        if (dialogueData.endDialogueLines.Length > dialogueIndex && dialogueData.endDialogueLines[dialogueIndex])
        {
            EndDialogue();
            return;
        }

        //Check if choices and display
        foreach(DialogueChoice dialogueChoice in dialogueData.dialogueChoices)
        {
            if (dialogueChoice.dialogueIndex == dialogueIndex)
            {
                DisplayChoices(dialogueChoice);
                return;
            }
        }

        if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            TypeLine();
        }
        else
        {
            EndDialogue();
        }
    }

    void TypeLine()
    {
        //TODO: Controlla se la riga deve essere arricchita
        if (dialogueData.enrich.Length > dialogueIndex && dialogueData.enrich[dialogueIndex])
        {
            //dialogueController.SetDialogueText(EnrichDialogueWithOllama(dialogueData.dialogueLines[dialogueIndex]));
        }else
        {
            dialogueController.SetDialogueText(dialogueData.dialogueLines[dialogueIndex]);
        }
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        dialogueController.ShowDialogueUI(false);
        dialogueController.SetDialogueText(string.Empty);
        //Riprendi il movimento
    }

    void DisplayChoices(DialogueChoice dialogueChoice)
    {
        for (int i = 0; i < dialogueChoice.choices.Length; i++)
        {
            int nextIndex = dialogueChoice.nextDialogueIndex[i];
            dialogueController.CreateChoiceButton (dialogueChoice.choices[i], () => ChooseOption(nextIndex));
        }
    }

    void ChooseOption(int nextIndex)
    {
        dialogueIndex = nextIndex;
        dialogueController.ClearChoices();
        TypeLine();
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
