using UnityEngine;

[CreateAssetMenu(fileName = "NPCBackgroundSO", menuName = "NPC Background")]
public class NPCBackgroundSO : ScriptableObject
{
    [TextArea(3, 10)]
    public string background;
    [TextArea(2, 5)]
    public string[] guardrails;
    //Eventi che possono o non possono essere influenti al fine della riformulazione.
    [TextArea(2, 5)]
    public string[] eventPerceptions;
    //Modifiche alle prossime risposte del NPC
    [TextArea(2, 5)]
    public string[] elicitedResponses;
}
