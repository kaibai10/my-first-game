using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSkillButton : MonoBehaviour
{
    public Image skillIcon;
    public int skillIndex;

    public void UpdateButtonInfo(Sprite skillicon)
    {
        Debug.Log("¸üÐÂÍ¼Ïñ");
        skillIcon.sprite = skillicon;
    }

    public void ActivedButton() 
    {
        if (SkillSelectSystem.instance.currentskillIndex != skillIndex)
        {
            SkillSelectSystem.instance.currentskillIndex = skillIndex;
            SkillSelectSystem.instance.UpdateCurrentSkillList();
        }
    }
}
