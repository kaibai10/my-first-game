using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : EnemiesBase
{
    private EnemyFSMBase currentState;  //当前状态

    public SkeletonMecanim skele;   //角色动画控制
    public Animator anim;

    private LeaderController lastHateTarget;    //上个仇恨对象
    public LeaderController[] allLeaders;       //场景中所有角色对象         可以使用一个场景单例，在场景开始时单独保存场景中的所有角色，避免频繁的从场景中
    private void Awake()
    {
        skele = gameObject.GetComponentInChildren<SkeletonMecanim>();
        anim = gameObject.GetComponentInChildren<Animator>();

        currentState = new EnemyFSMBase();
        currentState.enter(this);
    }

    void Start()
    {
        currentHealth = maxHealth;
        lastHateTarget = null;
        // 动态查找场景中所有 LeaderController 实例，自动填充 hateTable
        allLeaders = FindObjectsOfType<LeaderController>();
        foreach (var gameObject in allLeaders) //CharacterSelectSystem.instance.characterPrefabs
        {
            LeaderController leader = gameObject;
            hateTable[leader] = 0;
        }

        hateTarget = GetHateTarget();
    }

    private void Update()
    {
        currentState.update();

        HandleInput();
    }

    private void LateUpdate()
    {
        if (lastHateTarget != hateTarget)
            lastHateTarget = hateTarget;
    }

    void HandleInput() 
    {
        Debug.Log("检测按下");
        PlayerInput input = PlayerInput.DEFAULT;
        if (Input.GetKeyDown(KeyCode.J)) { input = PlayerInput.PRESS_J; Debug.Log("按下J"); }
        else if (Input.GetKeyUp(KeyCode.J)) { input = PlayerInput.RELEASE_J; Debug.Log("松开J"); }
        else if (Input.GetKeyDown(KeyCode.W)) { input = PlayerInput.PRESS_W; Debug.Log("按下W"); }
        else if (Input.GetKeyUp(KeyCode.W)) { input = PlayerInput.RELEASE_W; Debug.Log("松开W"); }
        if (input == PlayerInput.DEFAULT) return;

        EnemyFSMBase newState = currentState.handleInput(input);
        StateChange(newState);
    }

    public void StateChange(EnemyFSMBase enemyState) 
    {
        if (enemyState != null) 
        {
            currentState.exit();        //调用旧状态的退出函数
            currentState = enemyState;  //切换状态
            currentState.enter(this);   //切换状态后，调用新状态的enter函数。
        }
    }

    public LeaderController GetHateTarget() 
    {
        if (hateTable.Count < 1) return null;

        float maxHateVal = -1;
        LeaderController targetLeader = null;
        foreach (var table in hateTable) 
        {
            if (table.Value > maxHateVal && Vector3.Distance(transform.position, table.Key.transform.position) < hateRange) //找到仇恨范围内仇恨值最大的敌人
            {
                maxHateVal = table.Value;
                targetLeader = table.Key;
            }
        }

        return targetLeader;
    }

    //受伤
    public void TakeDamage(float damageToTake)
    {
        currentHealth -= damageToTake;
        if (currentHealth <= 0)
        {
            EnemySpawner.instance.RemoveInList(gameObject);
            Debug.Log("敌人：" + gameObject.name + "生命值降为零。");
            Destroy(gameObject);
        }
    }
}
