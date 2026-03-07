using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SkillActivation : MonoBehaviour
{
    public TMP_Text skillName;
    public Sprite skillIcon;

    private Skill selectedSkill;    //该按钮绑定的技能
    private bool is_FixedPosition;
    public LeaderController.skillType skilltype;  //技能是第几个技能
    private SkillType skillType;    //技能的类型
    GameObject t_Prefeb = null; //当前正在显示的预览物体
    private bool isAwaitingRelease; // 是否处于「等待释放」状态
    float angle;//旋转角度

    public void UpdataSelectedSkillInfo(Skill theSkill) 
    {
        skillName.text = theSkill.skillName;
        skillIcon = theSkill.icon;
        selectedSkill = theSkill;
        skillType = theSkill.selfSkillType;
    }

    private void Update()
    {
        if (isAwaitingRelease)
        {
            switch (skillType) 
            {
                case SkillType.POINT_TARGET:
                    SkillAnglePreview(); break;

                case SkillType.MOVABLE_AREA:
                    SkillPosPreview(); break;
            }
        }
    }

    //预览技能角度
    private void SkillAnglePreview()
    {
        Vector3 direction;//旋转目标向量

        //创建预览物体
        if (t_Prefeb == null)
        {
            if (MouseClickDetection.instance.currentLeader.targetPosition != Vector3.zero)
                t_Prefeb = Instantiate(selectedSkill.gameObject, MouseClickDetection.instance.currentLeader.targetPosition, Quaternion.identity);
            else
                t_Prefeb = Instantiate(selectedSkill.gameObject, MouseClickDetection.instance.currentLeader.transform.position, Quaternion.identity);
            t_Prefeb.SetActive(true);
        }

        //设置角度
        if (MouseClickDetection.instance.currentLeader.targetPosition != Vector3.zero)
        {
            direction = GetMousePos.instance.GetMousePosition() - MouseClickDetection.instance.currentLeader.targetPosition;
            angle = Vector3.SignedAngle(Vector3.right, direction, Vector3.forward);
            t_Prefeb.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        else
        {
            direction = GetMousePos.instance.GetMousePosition() - MouseClickDetection.instance.currentLeader.transform.position;
            angle = Vector3.SignedAngle(Vector3.right, direction, Vector3.forward);
            t_Prefeb.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        if (Input.GetMouseButtonDown(0))
        {
            MouseClickDetection.instance.currentLeader.haveSkillIsActivation = true;
            SkillCdController.instance.StartCd(selectedSkill, selectedSkill.skillStats[selectedSkill.spellAbility].cd);
            angle = Vector3.SignedAngle(Vector3.right, direction, Vector3.forward);
            MouseClickDetection.instance.currentLeader.nextSkill = skilltype;
            MouseClickDetection.instance.currentLeader.nextAngle = angle;

            Uncheck();
        }

        //取消选中
        if (Input.GetMouseButtonUp(1))
        {
            Uncheck();
            angle = 0;
        }
    }

    //技能位置设置
    private void SkillPosPreview()
    {
        Vector3 skillPosToRelease;
        //创建预览物体
        if (t_Prefeb == null)
        {
            Debug.Log("创建");
            t_Prefeb = Instantiate(selectedSkill.gameObject, GetMousePos.instance.GetMousePosition(), Quaternion.identity);
            t_Prefeb.GetComponent<Skill>().enabled = false; //预览状态不触发相应的技能逻辑
            t_Prefeb.SetActive(true);
        }
        else
        {
            Debug.Log("跟随");
            t_Prefeb.transform.position = GetMousePos.instance.GetMousePosition();//跟随鼠标
        }

        //再次点击后销毁预览预制体,并锁定技能释放位置
        if (Input.GetMouseButtonDown(0))
        {
            MouseClickDetection.instance.currentLeader.haveSkillIsActivation = true;//当前有技能在释放，不可再释放当前角色的其他技能
            SkillCdController.instance.StartCd(selectedSkill, selectedSkill.skillStats[selectedSkill.spellAbility].cd);//进入冷却
            skillPosToRelease = GetMousePos.instance.GetMousePosition();
            Debug.Log("skillPosToRelease:" + skillPosToRelease);
            MouseClickDetection.instance.currentLeader.nextSkill = skilltype;
            MouseClickDetection.instance.currentLeader.nextPosition = skillPosToRelease;
            MouseClickDetection.instance.currentLeader.nextPrefab = selectedSkill.gameObject;

            Uncheck();
        }

        //取消选中
        if (Input.GetMouseButtonUp(1))
            Uncheck();
    }

    //取消选中
    private void Uncheck()
    {
        Destroy(t_Prefeb);
        t_Prefeb = null;
        isAwaitingRelease = false;
        MouseClickEventsController.instance.selectedCharacter_Skill_Release = false;

    }

    public void ActivationSkill() 
    {
        if (selectedSkill.is_cd) 
        {
            Debug.Log("技能正在冷却");
            return;
        }

        if (selectedSkill == null)
        {
            Debug.LogWarning("没有选中技能");
            return;
        }

        // 根据技能释放类型分发处理
        switch (skillType)
        {
            case SkillType.POINT_TARGET:
                Activation_PointTargetSkill();
                break;

            case SkillType.UNIT_TARGET:
                Activation_UnitTarget();
                break;

            case SkillType.SELF:
                Activation_Self();
                break;

            case SkillType.FIXED_AREA:
                Activation_FixedArea();
                break;

            case SkillType.MOVABLE_AREA:
                Activation_MovableArea();
                break;
        }
    }

    public void Activation_PointTargetSkill() // 指向型技能释放（点地面）
    {
        if (!isAwaitingRelease) 
        {
            Debug.Log("指向性技能释放");
            isAwaitingRelease = true;
            MouseClickEventsController.instance.selectedCharacter_Skill_Release = true;
            //创建预览物体以确定方向
            SkillAnglePreview();
        }
    }

    public void Activation_UnitTarget() // 选取型技能释放（点单位）
    {
    
    }

    public void Activation_Self()   // 自身增益/无目标
    {

    }

    public void Activation_FixedArea()  // 范围型-不可拖动
    {

    }

    public void Activation_MovableArea() // 范围型-可拖动
    {
        if (!isAwaitingRelease)
        {
            Debug.Log("范围型-可拖动技能释放");
            isAwaitingRelease = true;
            MouseClickEventsController.instance.selectedCharacter_Skill_Release = true;
            //创建预览物体以确定方向
            SkillPosPreview();
        }
    }


}
