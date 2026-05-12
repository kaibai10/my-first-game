using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{
    public SkillData skillData;
    protected float durationTimer; //技能持续时间计时器
    protected float intervalTimer; //伤害间隔时间计时器
    public int skillSlot;   //技能所属槽位
    public bool isDestory;//是否需要被销毁
    [HideInInspector] public bool is_cd;//是否处于冷却状态
    [HideInInspector] public float timeCounter_cd = 0;//冷却计时器
    public LeaderController leader;
    public SkillController skillController;

    //获取技能范围内的敌人列表（由子物体上的 SkillTrigger 提供）
    public SkillTrigger skillTrigger;

    public virtual void Initialize(SkillData theSkillData) 
    {
        leader = GetComponentInParent<LeaderController>();
        skillController = GetComponentInParent<SkillController>();
        skillSlot = 0;
        skillData = theSkillData;
        durationTimer = skillData.stats[skillData.skillLevel].duration;
        intervalTimer = skillData.stats[skillData.skillLevel].attackInterval;   //攻击间隔
        skillTrigger = GetComponentInChildren<SkillTrigger>();
    }

    public virtual void Update() 
    {
        durationTimer -= Time.deltaTime;
        if (durationTimer < 0) 
        {
            FinishSkill();
            return;
        }

        ExecuteEffect();
    }

    // 执行技能（每种技能的具体效果）
    public abstract void ExecuteEffect();

    // 技能取消或结束
    public abstract void FinishSkill();
}

//技能执行上下文
public struct SkillContext 
{
    public Vector3 position;    //释放位置
    public float angle;         //释放角度
    public GameObject prefab;   //技能预制体
}