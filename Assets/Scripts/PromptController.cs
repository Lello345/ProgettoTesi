using System.Collections.Generic;
using UnityEngine;

public class PromptController : MonoBehaviour
{
    public static PromptController Instance { get; private set; }

    private string[] events;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
        Debug.Log("ciao sono prompt manager");
    }

    public string GeneratePrompt(string npcBackground, string[] elicitedResponses, string npcLine)
    {
        string eResponses = elicitedResponses?.Length > 0 ? string.Join("\n- ", elicitedResponses) : "";
        string eventPerception = events?.Length > 0 ? string.Join("\n- ", events) : "";
        string prompt = $"[PROFILO NPC & BACKGROUND]:\n{npcBackground}\n\n" +
                        $"[PERCEZIONE DEL MONDO E DEL GIOCATORE]:\n- {eventPerception}\n\n " +
                        $"[ELICITED RESPONSES]:\n- {eResponses}\n\n" +
                        $"[PROMEMORIA]: Resta rigorosamente fedele al background e alla percezione sopra indicati. Non aggiungere dettagli non presenti.\n\n" +
                        $"[DIALOGO DA RIFRASARE]:\n - {npcLine}";
        return prompt;
    }
}
