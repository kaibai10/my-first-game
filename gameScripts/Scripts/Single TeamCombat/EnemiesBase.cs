using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesBase : MonoBehaviour
{
    public float maxHealth, currentHealth;  //最大生命值/当前生命值
    public float moveSpeed;

    [Header("普通攻击范围/攻击伤害")]
    public float attackRange;
    public int attackAmount;

    [Header("仇恨范围/仇恨值/仇恨检测图层")]
    public float hateRange;
    public float hateValue;
    public LayerMask whatIsControllable;

    public Dictionary<LeaderController, float> hateTable = new Dictionary<LeaderController, float>();   //所有角色的仇恨表

    public LeaderController hateTarget;      //仇恨对象
    [HideInInspector] public LeaderController attackTarget;    //攻击对象
    [HideInInspector] public Vector3 targetDirection;    //当前敌人到攻击目标的方向
    [HideInInspector] public Collider2D[] hateRangechar;
    [HideInInspector] public Collider2D[] noramlAttackRangeChar;

    [Space]
    public ShowPath showPath;   //显示寻路路径
    public AStar AStar;         //A*寻路
    public List<Vector3> moveList = new List<Vector3>();    //路径点
}
