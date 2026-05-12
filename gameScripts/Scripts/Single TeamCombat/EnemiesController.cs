using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemiesController : EnemiesBase ,AllUnitAttributeModify
{
    private float distance; //当前物体与仇恨对象间的距离
    private LeaderController lastHateTarget;

    //public List<Skill> enemySkills;
    public SkeletonMecanim skele;
    public Animator anim;

    //性能优化
    private float detectionUpdateInterval = 0.2f; // 检测更新间隔
    private float detectionTimeCounter = 0f;

    public LeaderController[] allLeaders;

    private void Start()
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
    }

    private void Update()
    {
        detectionTimeCounter -= Time.deltaTime;
        if (detectionTimeCounter <= 0)  //间隔0.2s执行      
        {
            detectionTimeCounter = detectionUpdateInterval;
            hateTarget = GetHateTarget();

            if (lastHateTarget != hateTarget || hateTarget.anim.GetBool("Is_Moving")) //当仇恨目标改变 / 仇恨目标正常移动时,间隔获取路径点
            {
                //当仇恨对象不为空且攻击对象不在攻击范围以内时更新
                if (hateTarget != null && !anim.GetBool("Is_Attacking"))
                    GetMoveList();
            } 
        }

        if (hateTarget != null) 
        {
            Move(moveList);
            if (!anim.GetBool("Is_Moving"))
                Attack();
        }
    }

    private void LateUpdate()
    {
        if (lastHateTarget != hateTarget) 
        {
            lastHateTarget = hateTarget;
        }
    }

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

    void Attack() 
    {
        if (Vector3.Distance(transform.position, hateTarget.transform.position) <= attackRange)
        {
            anim.SetBool("Is_Attacking", true);
            //注：暂时未设置伤害逻辑TakeDamege
        }
        else 
        {
            anim.SetBool("Is_Attacking", false);
        }
    }

    private void Move(List<Vector3> moveList)
    {
        if (Vector3.Distance(transform.position, hateTarget.transform.position) > attackRange)
        {
            if (moveList.Count > 1)
            {
                Vector3 direction = (moveList[0] - transform.position).normalized;
                transform.position += moveSpeed * direction * Time.deltaTime;
                skele.skeleton.ScaleX = direction.x < 0 ? -1f : 1f;
                anim.SetBool("Is_Moving", true);
                if (Vector3.Distance(transform.position, moveList[0]) < 0.05f)
                {
                    moveList.RemoveAt(0);
                }
            }
            else
                anim.SetBool("Is_Moving", false);
        }
        else
            anim.SetBool("Is_Moving", false);
    }

    //获取路径点
    private void GetMoveList() 
    {
        //每次获取路径点前要清空上次已得到的路径点
        moveList.Clear();

        //获取世界位置的起始/结束点
        Vector3 startPos = transform.position;
        Vector3 endPos = hateTarget.transform.position;

        //转化为tilemap网格坐标
        Vector3Int startNode = MyGrid.instance.WorldToGridIndex(startPos);
        Vector3Int endNode = MyGrid.instance.WorldToGridIndex(endPos);

        startNode.z = height;
        endNode.z = hateTarget.GetComponent<LeaderController>().height;

        MyGridIndex startIdx = new MyGridIndex(startNode.x, startNode.y, startNode.z);
        MyGridIndex endIdx = new MyGridIndex(endNode.x, endNode.y, endNode.z);

        //寻找路径点
        List<MyNode> path = AStar.FindPath(startIdx, endIdx);
        for (int i = 1; i < path.Count; i++)
        {
            moveList.Add(path[i].pos);
        }

        //在游戏中显示路径（可启用）
        showPath.ShowPathLine(path);
    }

    //获取仇恨目标（取仇恨值最大且在仇恨范围的角色）
    private LeaderController GetHateTarget() 
    {
        float maxHate = -1;
        LeaderController hateTarget = null;
        foreach (var table in hateTable) 
        {
            if (table.Value > maxHate)      
            {
                if (Vector3.Distance(transform.position, table.Key.transform.position) <= hateRange)
                {
                    //distance = Vector3.Distance(transform.position, table.Key.transform.position);
                    maxHate = table.Value;
                    hateTarget = table.Key;
                }
            }
        }
        return hateTarget;
    }

    public void ApplyBuffModify(CharacterAttribute characterAttribute) 
    {
        if (characterAttribute.addAttack != 0) { attackDamage *= (1 + characterAttribute.addAttack); }
        if (characterAttribute.addHealth != 0) { maxHealth *= (1 + characterAttribute.addHealth); }
        if (characterAttribute.addMagicResistance != 0) { Debug.Log("修改法抗数值"); }
        if (characterAttribute.addArmorResistance != 0) { Debug.Log("修改物抗数值"); }
    }

    public void RemoveBuffModify(CharacterAttribute characterAttribute) 
    {
        if (characterAttribute.addAttack != 0) { attackDamage /= (1 + characterAttribute.addAttack); }
        if (characterAttribute.addHealth != 0) { maxHealth /= (1 + characterAttribute.addHealth); }
        if (characterAttribute.addMagicResistance != 0) { Debug.Log("撤销修改法抗数值"); }
        if (characterAttribute.addArmorResistance != 0) { Debug.Log("撤销修改物抗数值"); }
    }
}
