using UnityEngine;
using System.Collections.Generic;
using System.Text;

[CreateAssetMenu(fileName = "NewFreeChatStrategy", menuName = "AI Dialogue/Strategie/FreeChat")]
public class FreeChatStrategy : DialogueStrategySO
{
    private readonly string freeChatInstructions = 
        "ISTRUZIONI DI FORMATO:\n" +
        "Rispondi ESCLUSIVAMENTE con un oggetto JSON valido. Non aggiungere testo extra o markdown (no ```json).\n" +
        "Struttura JSON obbligatoria (la lista delle scelte DEVE essere vuota):\n" +
        "{\n" +
        "  \"npc_response\": \"La risposta testuale dell'NPC (massimo 2-3 frasi)\",\n" +
        "  \"player_choices\": []\n" +
        "}";

    public override string FormatPrompt(string NPCstory, string playerInput, List<string> history)
    {
        StringBuilder sb = new StringBuilder();

        // 1. Profilo e regole dell'NPC
        sb.AppendLine("=== PROFILO PERSONAGGIO ===");
        sb.AppendLine(NPCstory);
        sb.AppendLine();

        // 2. Istruzioni di output JSON
        sb.AppendLine(freeChatInstructions);
        sb.AppendLine();

        // 3. Cronologia
        sb.AppendLine("=== CRONOLOGIA DIALOGO ===");
        if (history != null && history.Count > 0)
        {
            foreach (string line in history)
            {
                sb.AppendLine(line);
            }
        }
        else
        {
            sb.AppendLine("Nessun dialogo precedente.");
        }
        sb.AppendLine();

        // 4. Input attuale
        sb.AppendLine($"Utente dice: '{playerInput}'");
        sb.AppendLine("Genera il JSON di risposta:");

        return sb.ToString();
    }
}
