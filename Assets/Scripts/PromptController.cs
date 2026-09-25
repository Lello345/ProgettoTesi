using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class PromptController : MonoBehaviour
{
    public static PromptController Instance { get; private set; }

    private List<string> events = new List<string>();

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public string GeneratePrompt(string npcBackground, string[] elicitedResponses, string npcLine)
    {
        string elicitedSection;
        if (elicitedResponses != null && elicitedResponses.Length > 0)
        {
            string eResponses = string.Join("\n- ", elicitedResponses);
            elicitedSection = $"[ELICITED RESPONSES] (Resta fedele all'emozione richiesta o menziona i fatti descritti):\n- {eResponses}\n\n";
        }
        else
        {
            elicitedSection = "[ELICITED RESPONSES]: Nessuna fornita in questo caso. Mantieni il significato letterale e il contenuto specifico della battuta originale, applicando solo la personalità di base dell'NPC allo stile. Non aggiungere reazioni, obiettivi o toni non richiesti.\n\n";
        }
        string eventPerception = events.Count > 0 ? string.Join("\n- ", events) : "";
        string prompt = $"[PROFILO NPC & BACKGROUND]:\n{npcBackground}\n\n" +
                        $"[PERCEZIONE DEL MONDO]:\n- {eventPerception}\n\n " +
                        elicitedSection +
                        $"[PROMEMORIA]: Resta rigorosamente fedele al background e alla percezione sopra indicati. Non aggiungere dettagli non presenti.\n\n" +
                        $"[DIALOGO DA RIFRASARE]:\n - {npcLine}";
        return prompt;
    }

    public void AddEventPerception(string eventP)
    {
        events.Add(eventP);
    }

    public void RemoveEventPerception(string eventP)
    {
        events.Remove(eventP);
    }
}
