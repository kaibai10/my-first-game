using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClickEventsController : MonoBehaviour
{
    public static MouseClickEventsController instance;
    private void Awake()
    {
        instance = this;
    }

    //各种鼠标事件
    public bool character_Move_Mouse;  //当前正使用鼠标控制角色移动
    public bool character_Move_Key;    //当前正使用键盘控制角色移动
    public bool selectedCharacter_Skill_Release;   //当前正控制角色技能释放位置
}
