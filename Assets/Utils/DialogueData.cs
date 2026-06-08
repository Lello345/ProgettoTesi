using UnityEngine;

using System.Collections.Generic;

[System.Serializable]
public class DialogueData
{
    // Questi nomi devono corrispondere IDENTICI alle chiavi JSON richieste nel prompt all'IA
    public string npc_response;
    public List<string> player_choices;

    public DialogueData()
    {
        player_choices = new List<string>();
    }

    // Costruttore di fallback in caso di errore o per la chat libera
    public DialogueData(string response)
    {
        this.npc_response = response;
        this.player_choices = new List<string>();
    }
}
