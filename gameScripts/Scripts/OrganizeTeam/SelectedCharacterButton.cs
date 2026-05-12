using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedCharacterButton : MonoBehaviour
{
    public int index;
    public Image illillustration;
    public GameObject teamSelectPanel;

    public void UpdateButton() 
    {
        if (SelectedCharacter.instance.leaderControllers[index] != null)
        {
            illillustration.sprite = SelectedCharacter.instance.leaderControllers[index].illustration;
        }
        else 
        {
            Debug.Log("按钮元素" + index + "为空");
        }
    }

    public void ActiveButton() 
    {
        teamSelectPanel.gameObject.SetActive(true);
        SelectedCharacter.instance.currentMakingCharacterIndex = index;
        if (SelectedCharacter.instance.leaderControllers[index] != null)
        {
            //对原有角色进行修改，将currentCareer和currentCharacter设为当前选中的角色的信息
            var leader = SelectedCharacter.instance.leaderControllers[index];
            CharacterSelectSystem.instance.currentCareer = leader.career;
            CharacterSelectSystem.instance.currentCharacterList = CharacterSelectSystem.instance.careerCharacter[leader.career];
            CharacterSelectSystem.instance.currentCharacter = leader;
            if (PageTransition.instance.isNextPage == false) PageTransition.instance.GoToNextPage();

            CharacterSelectSystem.instance.UpdateCareerList();  //更新角色选取列表
            CharacterSelectSystem.instance.RefreshCurrentCharacterDisplay(); 
        }
        else 
        {
            //选取新角色，重置currentCareer和currentCharacter
            CharacterSelectSystem.instance.currentCareer = "guards";
            CharacterSelectSystem.instance.currentCharacterList = CharacterSelectSystem.instance.careerCharacter["guards"];
            CharacterSelectSystem.instance.currentCharacter = CharacterSelectSystem.instance.currentCharacterList[0];
            if (PageTransition.instance.isNextPage == true) PageTransition.instance.GoToNextPage();
            
            CharacterSelectSystem.instance.UpdateCareerList();  //更新角色选取列表
            CharacterSelectSystem.instance.RefreshCurrentCharacterDisplay();
        }
        SelectedCharacter.instance.gameObject.SetActive(false);
    }
}
