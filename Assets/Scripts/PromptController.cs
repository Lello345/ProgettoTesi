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
    }

    public string GeneratePrompt(string npcBackground, string[] elicitedResponses, string npcLine)
    {
        string elicitedSection;
        if (elicitedResponses != null && elicitedResponses.Length > 0)
        {
            string eResponses = string.Join("\n- ", elicitedResponses);
            elicitedSection = $"[ELICITED RESPONSES] (RESTA FEDELE A QUESTI PUNTI NELLA RIFRASAZIONE):\n- {eResponses}\n\n";
        }
        else
        {
            elicitedSection = "[ELICITED RESPONSES]: Nessuna fornita in questo caso. Mantieni il significato letterale e il contenuto specifico della battuta originale, applicando solo la personalità di base dell'NPC allo stile. Non aggiungere reazioni, obiettivi o toni non richiesti.\n\n";
        }
        string eventPerception = events?.Length > 0 ? string.Join("\n- ", events) : "";
        string prompt = $"[PROFILO NPC & BACKGROUND]:\n{npcBackground}\n\n" +
                        $"[PERCEZIONE DEL MONDO E DEL GIOCATORE]:\n- {eventPerception}\n\n " +
                        elicitedSection +
                        $"[PROMEMORIA]: Resta rigorosamente fedele al background e alla percezione sopra indicati. Non aggiungere dettagli non presenti.\n\n" +
                        $"[DIALOGO DA RIFRASARE]:\n - {npcLine}";
        return prompt;
    }
}
