using System;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueView : MonoBehaviour
{
    [SerializeField] private NPCManager npcManager; // Riferimento al NPCManager per comunicare le scelte del giocatore
    private VisualElement wrapper;
    private VisualElement npcText;
    private VisualElement playerChoicesContainer;
    private TextField playerInputField;
    private Button[] playerButtons;
    [SerializeField] private UIDocument uiDocument;

    private void Start() {
        // Inizializza i riferimenti agli elementi UI
        var root = uiDocument.rootVisualElement;
        wrapper = root.Q<VisualElement>("Wrapper");
        npcText = root.Q<VisualElement>("Text");
        playerChoicesContainer = root.Q<VisualElement>("ButtonsBorder");
        playerInputField = root.Q<TextField>("TextField");
        playerButtons = new Button[3];
        playerButtons[0] = root.Q<Button>("Answer1");
        playerButtons[1] = root.Q<Button>("Answer2");
        playerButtons[2] = root.Q<Button>("Answer3");

        foreach (var button in playerButtons)
        {
            button.RegisterCallback<ClickEvent>(evt => OnPlayerChoiceClicked(evt, button));
            button.text = "";
        }
        playerInputField.RegisterCallback<NavigationSubmitEvent>(evt => OnPlayerInputSubmitted(evt));
        playerInputField.value = "";
        wrapper.style.display = DisplayStyle.None; // Nascondi il wrapper all'inizio
    }

    void Update()
    {
        // Per testare la UI, puoi chiamare npcManager.InitInteraction() quando premi un tasto specifico
        if (Input.GetKeyDown(KeyCode.Space))
        {
            npcManager.InitInteraction();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            wrapper.style.display = DisplayStyle.None; // Nascondi il wrapper quando premi ESC
        }
    }

    private async void OnPlayerInputSubmitted(NavigationSubmitEvent evt)
    {
        DisplayWaitingState(); // Mostra lo stato di attesa mentre si elabora l'input
        playerInputField.value = ""; // Pulisce il campo di input dopo l'invio
        DialogueData nextDialogueNode = await npcManager.SendChoiceAsync(playerInputField.value); // Invia l'input del giocatore al NPCManager
        ShowDialogue(nextDialogueNode); // Aggiorna la UI con la nuova risposta dell'NPC e le eventuali nuove scelte
    }

    private async void OnPlayerChoiceClicked(ClickEvent evt, Button button)
    {
        DisplayWaitingState(); // Mostra lo stato di attesa mentre si elabora la scelta
        foreach (var btn in playerButtons)
        {
            btn.text = ""; 
        }
        DialogueData nextDialogueNode = await npcManager.SendChoiceAsync(button.text); // Invia la scelta del giocatore al NPCManager
        ShowDialogue(nextDialogueNode);
    }

    public void ShowDialogue(DialogueData dialogueData)
    {
        wrapper.style.display = DisplayStyle.Flex; // Mostra il wrapper
        npcText.Q<Label>().text = dialogueData.npc_response; // Aggiorna il testo dell'NPC

        if (dialogueData.player_choices.Count == 3)
        {
            playerChoicesContainer.style.display = DisplayStyle.Flex; // Mostra i bottoni
            playerInputField.style.display = DisplayStyle.None; // Nascondi il campo di input
            // Aggiorna i bottoni con le scelte del giocatore
            for (int i = 0; i < playerButtons.Length; i++)
            {
                if (i < dialogueData.player_choices.Count)
                {
                    playerButtons[i].SetEnabled(true);
                    playerButtons[i].text = dialogueData.player_choices[i];
                }
                else
                {
                    playerButtons[i].SetEnabled(false);
                    playerButtons[i].text = "";
                }
            }
        } else
        {
            playerChoicesContainer.style.display = DisplayStyle.None; // Nascondi i bottoni se non ci sono 3 scelte
            playerInputField.style.display = DisplayStyle.Flex; // Mostra il campo di input
            playerInputField.value = ""; // Pulisce il campo di input
        }
    }

    private void DisplayWaitingState()
    {
        playerChoicesContainer.style.display = DisplayStyle.None;
        playerInputField.style.display = DisplayStyle.None;
        npcText.Q<Label>().text = "<i>Sta pensando...</i>";
    }
}
