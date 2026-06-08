using System;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueView : MonoBehaviour
{
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

        // Disabilita i bottoni all'inizio
        foreach (var button in playerButtons)
        {
            button.RegisterCallback<ClickEvent>(evt => OnPlayerChoiceClicked(evt, button));
            button.text = "";
        }
        playerInputField.RegisterCallback<NavigationSubmitEvent>(evt => OnPlayerInputSubmitted(evt));
        wrapper.style.display = DisplayStyle.None; // Nascondi il wrapper all'inizio
    }

    private void OnPlayerInputSubmitted(NavigationSubmitEvent evt)
    {
        playerInputField.value = ""; // Pulisce il campo di input dopo l'invio
    }

    private void OnPlayerChoiceClicked(ClickEvent evt, Button button)
    {
        foreach (var btn in playerButtons)
        {
            btn.SetEnabled(false); // Disabilita tutti i bottoni dopo la scelta
        }
    }

    public void ShowDialogue(string npcResponse, string[] playerChoices)
    {
        wrapper.style.display = DisplayStyle.Flex; // Mostra il wrapper
        npcText.Q<Label>().text = npcResponse; // Aggiorna il testo dell'NPC

        if (playerChoices.Length == 3)
        {
            playerChoicesContainer.style.display = DisplayStyle.Flex; // Mostra i bottoni
            playerInputField.style.display = DisplayStyle.None; // Nascondi il campo di input
            // Aggiorna i bottoni con le scelte del giocatore
            for (int i = 0; i < playerButtons.Length; i++)
            {
                if (i < playerChoices.Length)
                {
                    playerButtons[i].SetEnabled(true);
                    playerButtons[i].text = playerChoices[i];
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
}
