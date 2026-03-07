using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectButton : MonoBehaviour
{
    public Skill assignedSkill;
    public Image skillIcon;

    public void UpdateButtonInfo(Skill theSkill) 
    {
        assignedSkill = theSkill;
        skillIcon.sprite = theSkill.icon;
    }

    public void ActivedButton() 
    {
        CharacterSelectSystem.instance.currentCharacter.skills[SkillSelectSystem.instance.currentskillIndex] = assignedSkill;   //将技能赋值给角色
        SkillSelectSystem.instance.selectedSkillButtonList[SkillSelectSystem.instance.currentskillIndex].UpdateButtonInfo(assignedSkill.icon);
    }
}
