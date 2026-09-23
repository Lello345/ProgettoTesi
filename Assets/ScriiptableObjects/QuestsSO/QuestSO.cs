using System;
using UnityEngine;

[CreateAssetMenu(menuName ="Quests/Quest")]
public class Quest : ScriptableObject
{
    public string questID;
    public string questName;
    public string description;
    public bool IsCompleted = false;

    private void OnEnable()
    {
        IsCompleted = false;
    }
    private void OnValidate()
    {
        if(string.IsNullOrEmpty(questID))
        {
            questID = questName + Guid.NewGuid().ToString();
        }
    }
}
