using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectedCharacter : MonoBehaviour
{
    public static SelectedCharacter instance;
    private void Awake()
    {
        instance = this;
    }

    public List<LeaderController> leaderControllers;
    public List<SelectedCharacterButton> selectedCharacterButtons;
    public int currentMakingCharacterIndex;

    private bool characterSelectFinish = false;

    //选择完成的按钮
    public Button selectCompleteButton;

    public void Update()
    {
        ShowSelectCompleteButton();
    }

    public void UpdatePanel() 
    {
        foreach (SelectedCharacterButton button in selectedCharacterButtons) 
        {
            button.UpdateButton();
        }
    }

    public void AddInleaderControllers(LeaderController theLeader)
    {
        LeaderController lastLeader = leaderControllers[currentMakingCharacterIndex];
        if (lastLeader != null) 
        {
            CharacterSelectSystem.instance.careerCharacter[CharacterSelectSystem.instance.currentCareer].Add(lastLeader);
        }

        leaderControllers[currentMakingCharacterIndex] = theLeader;
        Debug.Log("在" + currentMakingCharacterIndex + "位置插入元素");

        //添加角色后将角色从待选择角色列表中移除，如果栏位不为空，则将原角色放回到待选列表中
        CharacterSelectSystem.instance.careerCharacter[CharacterSelectSystem.instance.currentCareer].Remove(theLeader);
        //更新角色列表
        CharacterSelectSystem.instance.UpdateCareerList();    
    }

    //检测显示进入地图的按钮是否可以被选中
    public void ShowSelectCompleteButton() 
    {
        characterSelectFinish = true;
        foreach (LeaderController leader in leaderControllers)
        {
            if (leader == null) { characterSelectFinish = false; continue; }
            else 
            {
                foreach (var skill in leader.skillcontroller.equippedSkills) 
                {
                    if (skill == null) { characterSelectFinish = false; continue; }
                }
            }
        }

        //设置按钮是否可选
        if (characterSelectFinish == true) 
            selectCompleteButton.interactable = true;
        else 
            selectCompleteButton.interactable = false;
    }
}
