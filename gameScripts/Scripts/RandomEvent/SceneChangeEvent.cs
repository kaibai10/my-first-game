using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RandomEvents/SceneChangeEvent")]
public class SceneChangeEvent : EventData
{
    public List<EventActionBase> actions;

    public override void Execute()
    {
        foreach (var action in actions)
            action.OnActive();
    }
}
