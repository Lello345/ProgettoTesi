using UnityEngine;
using System.Collections.Generic;
using System.Text;

[CreateAssetMenu(fileName = "NewDynamicJsonStrategy", menuName = "AI Dialogue/Strategie/DynamicJson")]
public class DynamicJsonStrategy : DialogueStrategySO
{
    private readonly string jsonInstructions = 
        "ISTRUZIONI DI FORMATO:\n" +
        "Rispondi ESCLUSIVAMENTE con un oggetto JSON valido. Non aggiungere spiegazioni o markdown (no ```json).\n" +
        "Struttura JSON obbligatoria:\n" +
        "{\n" +
        "  \"npc_response\": \"La battuta diretta dell'NPC in linea con la sua personalità (max 2-3 frasi)\",\n" +
        "  \"player_choices\": [\"Opzione 1\", \"Opzione 2\", \"Opzione 3\"]\n" +
        "}\n" +
        "REGOLE PER LE SCELTE DEL GIOCATORE (FONDAMENTALE):\n" +
        "- Le scelte DEVONO ESSERE BATTUTE DI DIALOGO DIRETTESI lette in PRIMA PERSONA dal giocatore (es: 'Mi interessa quella storia', 'Non ho tempo per i libri', 'Quanto costa questo volume?').\n" +
        "- NON generare categorie, argomenti o temi astratti (EVITA cose come 'Libri di storia', 'Racconti d'avventura').\n" +
        "- Genera ESATTAMENTE 3 battute verbali di risposta (max 5 parole ciascuna) che riflettono 3 atteggiamenti diversi:\n" +
        "  1. Risposta interessata / cortese\n" +
        "  2. Risposta neutrale / pragmatica\n" +
        "  3. Risposta sbrigativa / provocatoria";

    public override string FormatPrompt(string NPCstory, string playerInput, List<string> history)
    {
        StringBuilder sb = new StringBuilder();

        // 1. Profilo e regole dell'NPC
        sb.AppendLine("=== PROFILO PERSONAGGIO ===");
        sb.AppendLine(NPCstory);
        sb.AppendLine();

        // 2. Istruzioni di output JSON
        sb.AppendLine(jsonInstructions);
        sb.AppendLine();

        // 3. Cronologia delle battute precedenti
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

        // 4. Input attuale del giocatore
        sb.AppendLine($"L'ultimo input del Giocatore è: '{playerInput}'");
        sb.AppendLine("Genera il JSON di risposta:");

        return sb.ToString();
    }
}
