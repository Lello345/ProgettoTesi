using UnityEngine;

using System.Collections.Generic;

[System.Serializable]
public class DialogueData
{
    // Questi nomi devono corrispondere IDENTICI alle chiavi JSON richieste nel prompt all'IA
    public string npc_response;

    public DialogueData()
    {
    }

    // Costruttore di fallback in caso di errore o per la chat libera
    public DialogueData(string response)
    {
        this.npc_response = response;
    }
}
