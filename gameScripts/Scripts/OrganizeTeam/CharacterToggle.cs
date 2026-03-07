using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class CharacterToggle : MonoBehaviour
{
    public Image sideImage, illustration;
    public TMP_Text characterNameText;
    public LeaderController assignedCharacter;

    public void UpdateCharacter(LeaderController theCharacter)
    {
        characterNameText.text = theCharacter.characterName;
        assignedCharacter = theCharacter;
    }

    public void ActivedToggle(bool isOn) 
    {
        if (isOn) 
        {
            Debug.Log("点击characterToggle");
            CharacterSelectSystem.instance.currentCharacter = assignedCharacter;
            CharacterSelectSystem.instance.currentCharacterNameText.text = assignedCharacter.characterName; 
            illustration.sprite = assignedCharacter.illustration;

            //更换角色后顺势更新角色已选择的技能列表
            for (int i = 0; i < 3; i++) 
            {
                Debug.Log("角色更新，更换全部列表图像");
                if (assignedCharacter.skills[i] != null)
                {
                    Debug.Log("非空调用");
                    SkillSelectSystem.instance.selectedSkillButtonList[i].UpdateButtonInfo(assignedCharacter.skills[i].icon);
                }
                else
                {
                    Debug.Log("空调用");
                    SkillSelectSystem.instance.selectedSkillButtonList[i].UpdateButtonInfo(null);
                }
            }
        }
    }
}
