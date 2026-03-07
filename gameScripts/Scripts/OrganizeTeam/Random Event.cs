using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//[CreateAssetMenu(fileName = "NewRandomEvent", menuName = "RandomEvent/New Event")]
public class RandomEvent : MonoBehaviour
{
    [TextArea(3, 10)] // 编辑器中多行文本
    public string eventDescription; // 事件描述
    public List<float> weights; //权重
    public int currentEventStage = 0;//当前事件阶段

    public List<Random1EventChoice> choices; // 三个选择项
}


[System.Serializable]
public class Random1EventChoice
{
    public string choiceText; // 选择项文本
    public UnityEvent action; // 可能的动作列表
}