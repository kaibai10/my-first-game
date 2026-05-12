using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RandomEvents/PreBattleEvent")]
public class PreBattleEvent : EventData
{
    public List<EventActionBase> actions;

    public override void Execute() 
    {
        foreach (var action in actions) 
            action.OnActive();
    }
}
