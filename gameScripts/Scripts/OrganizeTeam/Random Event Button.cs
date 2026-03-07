using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RandomEventButton : MonoBehaviour
{
    public TMP_Text eventDescription;
    public RandomEvent assignedEvent;
    public int index;

    public void UpdateButtonInfo(RandomEvent theEvent) 
    {
        eventDescription.text = theEvent.choices[index].choiceText;
        assignedEvent = theEvent;
    }

    public void ActiveButton() 
    {
        assignedEvent.choices[index].action.Invoke();
        Debug.Log("执行了事件" + assignedEvent.name + "的第" + index + "个选项");
    }
}
