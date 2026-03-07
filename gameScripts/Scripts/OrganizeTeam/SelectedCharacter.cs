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
        leaderControllers[currentMakingCharacterIndex] = theLeader;
        Debug.Log("在" + currentMakingCharacterIndex + "位置插入元素");

        //添加角色后将角色从待选择角色列表中移除
        //CharacterSelectSystem.instance.careerCharacter[CharacterSelectSystem.instance.currentCareer].Remove(theLeader);  出问题是因为按钮的index不再更新
        //更新角色列表
         CharacterSelectSystem.instance.UpdateCareerList();    
    }

    //检测显示进入地图的按钮是否可以被选中
    public void ShowSelectCompleteButton() 
    {
        bool showLeaderControllers = true;
        foreach (LeaderController cha in leaderControllers) 
        {
            if (cha == null)
                showLeaderControllers = false;
        }

        if (showLeaderControllers == true) 
        {
            selectCompleteButton.interactable = true;
        }
    }

    //显示地图
    public void GoToSampleScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
