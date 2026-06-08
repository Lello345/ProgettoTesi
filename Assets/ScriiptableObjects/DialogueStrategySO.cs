using UnityEngine;
using System.Collections.Generic;

public abstract class DialogueStrategySO : ScriptableObject, IDialogueStrategy
{
    public abstract string FormatPrompt(string NPCstory, string playerInput, List<string> history);
}

