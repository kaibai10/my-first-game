using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public List<SkillStats> skillStats;
    public string skillName, career;
    public int spellAbility;//施法能力(等级)
    public Sprite icon;//升级图标
    public bool isDestory;//是否需要被销毁
    [HideInInspector]
    public bool is_cd;//是否处于冷却状态
    [HideInInspector]
    public float timeCounter_cd = 0;//冷却计时器
    public SkillType selfSkillType;
}

[System.Serializable]
public class SkillStats 
{
    public int amount;
    public float damage, timeBetweenAttacks, duration, cd;
    public string skillDescription;

    public float nengliang;//升下一级消耗
}

public enum SkillType
{
    POINT_TARGET,          // 指向型（点地面）
    UNIT_TARGET,           // 选取型（点单位）
    SELF,                 // 自身增益/无目标
    FIXED_AREA,            // 范围型-不可拖动
    MOVABLE_AREA,          // 范围型-可拖动
    NULL                   //未选择技能
}