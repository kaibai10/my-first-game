using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectButton : MonoBehaviour
{
    public SkillBase assignedSkill;
    public Image skillIcon;

    public void UpdateButtonInfo(SkillBase theSkill) 
    {
        assignedSkill = theSkill;
        skillIcon.sprite = theSkill.skillData.icon;
    }

    public void ActivedButton() 
    {
        CharacterSelectSystem.instance.currentCharacter.GetComponent<SkillController>().equippedSkills[SkillSelectSystem.instance.currentskillIndex] = assignedSkill;
        SkillSelectSystem.instance.selectedSkillButtonList[SkillSelectSystem.instance.currentskillIndex].UpdateButtonInfo(assignedSkill.skillData.icon);
    }
}
