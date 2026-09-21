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
    private enum QuestState
    {
        NotStarted,
        InProgress,
        Completed
    }
    private QuestState questState = QuestState.NotStarted;
    
    [Header("Riferimenti di Sistema")]
    [SerializeField] private OllamaModelHandler modelHandler; // Il gestore della connessione a Ollama

    void Start()
    {
        dialogueController = DialogueController.Instance;
    }

    public bool CanInteract()
    {
        return true;
    }
    public void Interact()
    {
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
        //Sync quest data
        SyncQuestState();
        //Set dialogue line based on quest state
        switch (questState)
        {
            case QuestState.NotStarted:
                dialogueIndex = 0;
                break;
            case QuestState.InProgress:
                dialogueIndex = dialogueData.questInProgressIndex;
                break;
            case QuestState.Completed:
                dialogueIndex = dialogueData.questCompletedIndex;
                break;
        }

        isDialogueActive = true;

        dialogueController.SetNpcInfo(dialogueData.npcName, dialogueData.npcPortrait);

        dialogueController.ShowDialogueUI(true);
        //TODO: Stoppa il movimento

        TypeLine();
    }

    private void SyncQuestState()
    {
        if (dialogueData.quest == null) return;

        string questID = dialogueData.quest.questID;

        if (QuestController.Instance.IsQuestCompleted(questID))
        {
            questState = QuestState.Completed;
        }
        else if (QuestController.Instance.IsQuestActive(questID))
        {
            questState = QuestState.InProgress;
        }
        else
        {
            questState = QuestState.NotStarted;
        }

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

        //TODO: Controlla completamento di quest

        //TODO: Controlla arricchimento della riga

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
        /*TODO: Controlla se la riga deve essere arricchita
        if (dialogueData.enrich.Length > dialogueIndex && dialogueData.enrich[dialogueIndex])
        {
            dialogueController.SetDialogueText(EnrichDialogueWithOllama(dialogueData.dialogueLines[dialogueIndex]));
        }else
        {
            dialogueController.SetDialogueText(dialogueData.dialogueLines[dialogueIndex]);
        }
        */
        dialogueController.SetDialogueText(dialogueData.dialogueLines[dialogueIndex]);
    }

    void DisplayChoices(DialogueChoice dialogueChoice)
    {
        for (int i = 0; i < dialogueChoice.choices.Length; i++)
        {
            int nextIndex = dialogueChoice.nextDialogueIndex[i];
            bool givesQuest = dialogueChoice.givesQuest[i];
            dialogueController.CreateChoiceButton (dialogueChoice.choices[i], () => ChooseOption(nextIndex, givesQuest));
        }
    }

    void ChooseOption(int nextIndex, bool givesQuest)
    {
        if (givesQuest)
        {
            QuestController.Instance.AcceptQuest(dialogueData.quest);
            questState = QuestState.InProgress;
        }
        dialogueIndex = nextIndex;
        dialogueController.ClearChoices();
        TypeLine();
    }

    /*
    private async Task<string> EnrichDialogueWithOllama(string npcLine)
    {
        yield return null;
    }
    */
    
    private DialogueData ParseOllamaResponse(string rawResponse)
    {
        try
        {   
            DialogueData data = JsonUtility.FromJson<DialogueData>(rawResponse);

            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[NPCManager] Errore nel parsing del JSON di Ollama: {e.Message}");
            // Fallback per non bloccare il gioco in caso di allucinazione del modello
            return new DialogueData("...Non riesco a concentrarmi, la mia mente è offuscata... (Errore Generazione IA)");
        }
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        dialogueController.ShowDialogueUI(false);
        dialogueController.SetDialogueText(string.Empty);
        //Riprendi il movimento
    }
}
