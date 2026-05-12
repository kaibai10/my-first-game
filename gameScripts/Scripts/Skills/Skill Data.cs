using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "MyGame/SkillData")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public Sprite icon;
    public int skillLevel;
    public bool isFollowLeader;     //是否跟随父物体移动
    public bool skillFinishInCD;    //是否技能结束后才进入cd
    public List<SkillStats> stats;

    public GameObject skillPrefab; // 技能对应的预制体（包含范围检测）
    public SkillType skillType;
}

[System.Serializable]
public class SkillStats
{
    public int attackAmount;    //可攻击敌人的数量
    public float damage, attackInterval, duration, cd;  //伤害值/攻击间隔/持续时间/冷却时间
    public string skillDescription;//技能描述
    public float nextLevelCost;//升下一级花费
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