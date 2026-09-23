using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class NPCManager : MonoBehaviour, IInteractable
{
    [Header("Configurazione Dati")]
    private DialogueController dialogueController;
    private PromptController promptController;
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
    public async Task Interact()
    {
        if(dialogueData == null)
        {
            return;
        }

        if (!isDialogueActive)
        {
            await StartDialogue();
        }
        else
        {
            await NextLine();
        }
    }

    async Task StartDialogue()
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


        await TypeLineAsync();
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
    async Task NextLine()
    {
        //Pulisce le scelte se presenti in precedenza
        dialogueController.ClearChoices();

        //Controlla se a questa linea di dialogo deve mostrare delle linee di dialogo e le mostra
        foreach(DialogueChoice dialogueChoice in dialogueData.dialogueChoices)
        {
            if (dialogueChoice.dialogueIndex == dialogueIndex)
            {
                await DisplayChoices(dialogueChoice);
                return;
            }
            //Controlla se a questa linea di dialogo si deve completare una quest e la completa
            QuestCompletion questCompletition = dialogueChoice.questCompletion;
            if (questCompletition.questToComplete != null && questCompletition.dialogueIndexThatCompletesQuest == dialogueIndex)
            {
                QuestController.Instance.CompleteQuest(questCompletition.questToComplete.questID);
                break;
            }
        }
        //Controlla se deve finire il dialogo
        if (dialogueData.endDialogueLines.Length > dialogueIndex && dialogueData.endDialogueLines[dialogueIndex])
        {
            EndDialogue();
            return;
        }

        //Procede normalmente
        if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            await TypeLineAsync();
        }
        else
        {
            EndDialogue();
        }
    }

    async Task TypeLineAsync()
    {
        foreach (Enrichment enrichment in dialogueData.enrich)
        {
            if (enrichment.dialogueIndex == dialogueIndex)
            {
                if (enrichment.addElicitedResponses.Length > 0){
                    backgroundData.elicitedResponses = backgroundData.elicitedResponses.Union(enrichment.addElicitedResponses).ToArray();
                }
                // 1. Mostra subito un placeholder
                dialogueController.SetDialogueText("Pensando...");

                // 2. Attende la risposta da Ollama
                string enrichedText = await EnrichDialogueWithOllama(dialogueData.dialogueLines[dialogueIndex]);
                
                backgroundData.elicitedResponses = backgroundData.elicitedResponses.Except(enrichment.addElicitedResponses).ToArray();
                // 3. Aggiorna il testo finale
                dialogueController.SetDialogueText(enrichedText);
                return;
                
            }
        }
        dialogueController.SetDialogueText(dialogueData.dialogueLines[dialogueIndex]);
    }

    async Task DisplayChoices(DialogueChoice dialogueChoice)
    {
        for (int i = 0; i < dialogueChoice.choices.Length; i++)
        {
            bool isCompletionChoice = dialogueChoice.questCompletion.choiceIndexThatCompletesQuest == i;
            string questID = dialogueChoice.questCompletion.questToComplete != null ? dialogueChoice.questCompletion.questToComplete.questID : "";
            if (isCompletionChoice && !(QuestController.Instance.IsQuestActive(dialogueChoice.questCompletion.questToComplete.questID) && !QuestController.Instance.IsQuestCompleted(dialogueChoice.questCompletion.questToComplete.questID)))
            {
                
            }else
            {
                int nextIndex = dialogueChoice.nextDialogueIndex[i];
                bool givesQuest = dialogueChoice.givesQuest[i];
                dialogueController.CreateChoiceButton (dialogueChoice.choices[i], async () => await ChooseOption(nextIndex, givesQuest));
            }
            
        }
    }

    async Task ChooseOption(int nextIndex, bool givesQuest)
    {
        if (givesQuest)
        {
            QuestController.Instance.AcceptQuest(dialogueData.quest);
            questState = QuestState.InProgress;
        }
        dialogueIndex = nextIndex;
        dialogueController.ClearChoices();
        await TypeLineAsync();
    }

    
    private async Task<string> EnrichDialogueWithOllama(string npcLine)
    {
        // 1. Prepara la richiesta (Genera il prompt)
        string fullPrompt = PromptController.Instance.GeneratePrompt(backgroundData.background, backgroundData.elicitedResponses, npcLine);
        // 2. Invia la richiesta a Ollama e attendi la risposta
        string rawResponse = await modelHandler.SendPromptAsync(fullPrompt);
        // 3. Analizza la risposta JSON e ottieni il testo arricchito
        DialogueData enrichedData = ParseOllamaResponse(rawResponse);
        // 4. Restituisci il testo arricchito
        return enrichedData.npc_response;
    }
    
    
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
