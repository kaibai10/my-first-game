using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MoveState : EnemyFSMBase
{
    public AStar aStar;
    public ShowPath showPath;
    public List<Vector3> moveList = new List<Vector3>();

    private float detectionUpdateInterval = 0.2f; // 检测更新间隔
    private float detectionTimeCounter = 0f;

    public MoveState(Enemy enemy) { this.enemy = enemy; }
    public override void enter(Enemy theEnemy)
    {
        Debug.Log("进入移动状态");
        enemy.anim.SetBool("Is_Moving", true);
        detectionTimeCounter = detectionUpdateInterval;

        aStar = enemy.AStar;
        if (aStar == null) { Debug.Log("aStar为空"); }
        showPath = enemy.showPath;
        if (showPath == null) { Debug.Log("showPath为空"); }
        
    }

    public override EnemyFSMBase handleInput(PlayerInput input) 
    {
        if (input == PlayerInput.PRESS_J)
        {
            Debug.Log("移动状态切换至攻击状态");
            return new AttackState(enemy);
        }

        return null;
    }

    public override void update()
    {
        if (Vector3.Distance(enemy.transform.position, enemy.hateTarget.transform.position) + 1 < enemy.attackRange) 
        {
            enemy.StateChange(new AttackState(enemy));
            return;
        }

        Debug.Log("持续移动");
        detectionTimeCounter -= Time.deltaTime;
        if (detectionTimeCounter < 0)           //间隔0.2s更新一次路径
        {
            Debug.Log("检测路径");
            detectionTimeCounter = detectionUpdateInterval;
            enemy.hateTarget = enemy.GetHateTarget();
            if (enemy.hateTarget != null) 
                GetMoveList();
        }

        if (enemy.hateTarget != null)       //目标存在则移动
            Move(moveList);
        else Debug.Log("hateTarget为空");


    }

    public override void exit()
    {
        Debug.Log("退出移动状态");
        enemy.anim.SetBool("Is_Moving", false);
    }

    private void Move(List<Vector3> moveList) 
    {
        if (moveList.Count > 1) 
        {
            Vector3 dir = (moveList[0] - enemy.transform.position).normalized;
            enemy.transform.position += dir * enemy.moveSpeed * Time.deltaTime;
            enemy.skele.skeleton.ScaleX = dir.x < 0 ? -1f : 1f;

            if (Vector3.Distance(enemy.transform.position, moveList[0]) < 0.05f)
                moveList.RemoveAt(0);
        }
    }

    //获取路径点
    private void GetMoveList()
    {
        //每次获取路径点前要清空上次已得到的路径点
        moveList.Clear();

        //获取世界位置的起始/结束点
        Vector3 startPos = enemy.transform.position;
        Vector3 endPos = enemy.hateTarget.transform.position;

        //转化为tilemap网格坐标
        Vector3Int startNode = MyGrid.instance.WorldToGridIndex(startPos);
        Vector3Int endNode = MyGrid.instance.WorldToGridIndex(endPos);

        startNode.z = enemy.height;
        endNode.z = enemy.hateTarget.GetComponent<LeaderController>().height;

        MyGridIndex startIdx = new MyGridIndex(startNode.x, startNode.y, startNode.z);
        MyGridIndex endIdx = new MyGridIndex(endNode.x, endNode.y, endNode.z);

        //寻找路径点
        List<MyNode> path = aStar.FindPath(startIdx, endIdx);
        for (int i = 1; i < path.Count; i++)
        {
            moveList.Add(path[i].pos);
        }

        //在游戏中显示路径（可启用）
        showPath.ShowPathLine(path);
    }
}
