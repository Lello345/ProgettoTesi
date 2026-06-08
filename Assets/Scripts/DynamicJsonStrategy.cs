using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "NewDynamicJsonStrategy", menuName = "AI Dialogue/Strategie/DynamicJson")]
public class DynamicJsonStrategy : DialogueStrategySO
{
    private readonly string jsonInstructions = 
        "RISPONDI ESCLUSIVAMENTE IN FORMATO JSON.\n" +
        "Non aggiungere introduzioni, note o blocchi di codice markdown (no ```json).\n" +
        "Il JSON deve seguire tassativamente questa struttura:\n" +
        "{\n" +
        "  \"npc_response\": \"La battuta o risposta dell'NPC in linea con il suo carattere\",\n" +
        "  \"player_choices\": [\"Opzione di risposta A per il player\", \"Opzione B\", \"Opzione C\"]\n" +
        "}\n" +
        "Genera esattamente 3 opzioni per il giocatore, coerenti con la conversazione.";

    public override string FormatPrompt(string NPCstory, string playerInput, List<string> history)
    {
        // Ricostruiamo la cronologia per dare contesto all'IA
        string context = history.Count > 0 ? string.Join("\n", history) : "Nessun dialogo precedente.";

        return $"{NPCstory}\n\n" +
               $"{jsonInstructions}\n\n" +
               $"--- CRONOLOGIA DIALOGO ---\n{context}\n" +
               $"L'ultimo input del Giocatore è: '{playerInput}'\n" +
               $"Genera il JSON di risposta:";
    }
}
