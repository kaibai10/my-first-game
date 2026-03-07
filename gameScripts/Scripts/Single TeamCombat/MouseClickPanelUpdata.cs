using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;

public class MouseClickPanelUpdata : MonoBehaviour
{
    public static MouseClickPanelUpdata instance;

    private void Awake()
    {
        instance = this;
    }

    public TMP_Text character_Name;
    public Image character_Image;
    [SerializeField]
    private LeaderController leader;
    public List<SkillActivation> skillSelectButtons;

    // Update is called once per frame
    void Update()
    {
        if (MouseClickDetection.instance.currentLeader != null)
        {
            leader = MouseClickDetection.instance.currentLeader;
            UpdataNameText(leader);
            for (int i = 0; i < skillSelectButtons.Count; i++)
            {
                skillSelectButtons[i].UpdataSelectedSkillInfo(MouseClickDetection.instance.currentLeader.skills[i]);
            }
        }

        if (leader.haveSkillIsActivation)
        {
            foreach (SkillActivation button in skillSelectButtons) //为true设置按钮状态为不可点击
            {
                button.GetComponent<Button>().interactable = false;
            }
        }
        else
        {
            foreach (SkillActivation button in skillSelectButtons) //为flase设置按钮状态为可点击
            {
                button.GetComponent<Button>().interactable = true;
            }
        }
    }

    void UpdataNameText(LeaderController theLeader) 
    {
        character_Name.text = theLeader.characterName;
        character_Image.sprite = theLeader.sideImage;
    }
}
