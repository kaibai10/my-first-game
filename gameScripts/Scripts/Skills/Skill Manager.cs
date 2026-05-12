using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [HideInInspector] public TMP_Text skillName;
    [HideInInspector] public Sprite skillIcon;
    public int skillSlot;   //该按钮对应的技能槽
    private SkillController skillController;
    private SkillBase selectedSkill;    //该按钮绑定的技能
    GameObject t_Prefeb = null; //当前正在显示的预览物体
    private bool isAwaitingRelease; // 是否处于「等待释放」状态

    //更新按钮信息
    public void UpdataSelectedSkillInfo(SkillBase theSkillBase, SkillController controller)
    {
        skillName.text = theSkillBase.skillData.skillName;
        skillIcon = theSkillBase.skillData.icon;
        selectedSkill = theSkillBase;
        skillController = controller;
    }

    public void ActivetionButton() 
    {
        skillController.CastSkillByKey(skillSlot);
        Debug.Log("触发技能"+skillSlot);
    }
}
