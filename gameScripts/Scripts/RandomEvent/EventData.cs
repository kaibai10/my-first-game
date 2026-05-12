using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EventData : ScriptableObject
{
    public string eventID;  //标识符
    public string eventName;
    [TextArea] public string eventDescription;
    public EventType eventType;
    [Range(0, 1)] public float weight; //权重

    public List<EventData> prerequisites;  //前置条件，之前的事件全触发后才会触发

    [Header("宝箱交互事件")] public UnityEvent randomEvent;
    //事件的具体实现效果（由子类实现）
    public abstract void Execute();
}

public enum EventType 
{
    PreBattleEvent,     //进入战斗前可能出现的事件的类型
    SceneChangeEvent,   //进入场景时可能出现的事件的类型
}