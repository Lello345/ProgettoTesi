using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "NewFreeChatStrategy", menuName = "AI Dialogue/Strategie/FreeChat")]
public class FreeChatStrategy : DialogueStrategySO
{
    private readonly string freeChatInstructions = 
        "RISPONDI ESCLUSIVAMENTE IN FORMATO JSON.\n" +
        "Non aggiungere testo extra o markdown.\n" +
        "Il JSON deve seguire questa struttura (con la lista delle scelte vuota):\n" +
        "{\n" +
        "  \"npc_response\": \"La tua risposta testuale libera\",\n" +
        "  \"player_choices\": []\n" +
        "}";

    public override string FormatPrompt(string NPCstory, string playerInput, List<string> history)
    {
        string context = history.Count > 0 ? string.Join("\n", history) : "Nessun dialogo precedente.";

        return $"{NPCstory}\n\n" +
               $"{freeChatInstructions}\n\n" +
               $"--- CRONOLOGIA DIALOGO ---\n{context}\n" +
               $"Utente dice: {playerInput}\n" +
               $"Genera il JSON di risposta:";
    }
}
