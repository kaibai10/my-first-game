using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RandomEventController : MonoBehaviour
{
    public List<RandomEvent> repeatableEnevts;//可重复出现的事件列表
    public List<RandomEvent> norepeatableEnevts;//不可重复出现的事件列表

    public List<RandomEvent> happenedEvents;//当前发生的事件列表
    public int selectedEventIndex;
    public List<RandomEventButton> randomEventButtons;
    public TMP_Text eventDescriptionText;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.K)) 
        {
            happenedEvents.Clear();
            for (int i = 0; i < 3; i++)
                RandomEventHappend();

            selectedEventIndex = 1;
            UpdateAllEventButtons();
        }
    }
    
    void RandomEventHappend() 
    {
        RandomEvent currentEvent = null;
        float totalWeight = 0f;

        //计算总权重
        foreach (var theEvent in repeatableEnevts) 
            totalWeight += theEvent.weights[theEvent.currentEventStage];
        foreach (var theEvent in norepeatableEnevts)
            totalWeight += theEvent.weights[theEvent.currentEventStage];

        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        //遍历积累权重，找到命中的事件
        foreach (var theEvent in repeatableEnevts) 
        {
            cumulativeWeight += theEvent.weights[theEvent.currentEventStage];

            if (cumulativeWeight >= randomValue) 
            {
                currentEvent = theEvent;
                break;
            }
        }
        foreach (var theEvent in norepeatableEnevts)
        {
            cumulativeWeight += theEvent.weights[theEvent.currentEventStage];

            if (cumulativeWeight >= randomValue)
            {
                currentEvent = theEvent;

                //若命中事件为不可重复事件则需额外操作
                theEvent.currentEventStage++;
                if (theEvent.currentEventStage >= theEvent.weights.Count) 
                {
                    norepeatableEnevts.Remove(theEvent);
                }
            }
        }

        happenedEvents.Add(currentEvent);   //注：选择事件后需要清空,还需防止三个随机事件中有相同的事件
        Debug.Log("RandomEventHappend执行完毕");
        //添加按钮事件，更新按钮信息
    }

    //跳转到下个事件项
    public void ShowNextEvent() 
    {
        selectedEventIndex = (selectedEventIndex + 1) % 3;
        UpdateAllEventButtons();
    }
    //返回到上一个事件项
    public void ShowPreviousEvent() 
    {
        selectedEventIndex = (selectedEventIndex + 2) % 3; //(selectedEventIndex - 1 + 3) % 3;
        UpdateAllEventButtons();
    }

    //更换显示的事件时，更新所有button的显示信息，以及对事件的描述信息
    void UpdateAllEventButtons() 
    {
        foreach (RandomEventButton button in randomEventButtons)
        {
            button.UpdateButtonInfo(happenedEvents[selectedEventIndex]);
        }
        eventDescriptionText.text = happenedEvents[selectedEventIndex].eventDescription;
    }
}
