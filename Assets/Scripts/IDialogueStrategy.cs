using UnityEngine;
using System.Collections.Generic;

public interface IDialogueStrategy
{
    string FormatPrompt(string NPCstory, string playerInput, List<string> history);
}
