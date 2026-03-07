using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventChoiceController : MonoBehaviour
{
    //商人随机事件
    public void businessman_eventChoice1() 
    {
        Debug.Log("businessman_eventChoice1");
    }
    public void businessman_eventChoice2()
    {
        Debug.Log("businessman_eventChoice2");
    }
    public void businessman_eventChoice3()
    {
        Debug.Log("businessman_eventChoice3");
    }

    //遭遇敌人随机事件
    public void encounter_eventChoice1() 
    {
        Debug.Log("遭遇敌人a");
    }
    public void encounter_eventChoice2()
    {
        Debug.Log("遭遇敌人b");
    }
    public void encounter_eventChoice3()
    {
        Debug.Log("遭遇敌人c");
    }

    //随机debuff事件
    public void debuff_eventChoice1() 
    {
        Debug.Log("所有敌人攻击力增加10%");
    }
    public void debuff_eventChoice2()
    {
        Debug.Log("所有敌人防御力增加10%");
    }
    public void debuff_eventChoice3()
    {
        Debug.Log("所有敌人生命值增加10%");
    }

    //随机buff事件
    public void buff_eventChoice1() 
    {
        Debug.Log("获得了医疗队的额外物资，全体治疗量增加10%");
    }
    public void buff_eventChoice2()
    {
        Debug.Log("得到了队伍a已经抵达的信息，成员们士气大增，全体攻击力增加10%");
    }
    public void buff_eventChoice3()
    {
        Debug.Log("击杀了一位敌人的指挥官，敌人全体属性减少10%");
    }

    //流浪者队伍事件
    public void wanderers_eventChoice1() 
    {
        Debug.Log("你们不想伤害这群流浪者，同时为了自身安危，你们选择恐吓他们，把他们逼退");
    }
    public void wanderers_eventChoice2()
    {
        Debug.Log("战斗！");
    }
    public void wanderers_eventChoice3()
    {
        Debug.Log("为了减少这群流浪者的顾虑，你们选择派出一名队员与之谈判。");
    }
}
