using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestMove : MonoBehaviour
{
    public ShowPath showPath;
    public AStar AStar;
    public float moveSpeed = 4.0f;
    public List<Vector3> moveList = new List<Vector3>();

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            //每次获取新路径点时先清除现有路径列表
            moveList.Clear();

            //获取世界位置的起始/结束点
            Vector3 startPos = transform.position;
            Vector3 endPos = GetMousePos.instance.GetMousePosition();

            //转化为tilemap网格坐标
            Vector2Int startNode = MyGrid.instance.WorldToGridIndex(startPos);
            Vector2Int endNode = MyGrid.instance.WorldToGridIndex(endPos);

            MyGridIndex startIdx = new MyGridIndex(startNode.x, startNode.y);
            MyGridIndex endIdx = new MyGridIndex(endNode.x, endNode.y);

            List<MyNode> path = AStar.FindPath(startIdx, endIdx);

            for (int i = 0; i < path.Count; i++) 
            {
                moveList.Add(path[i].pos);
            }

            showPath.ShowPathLine(path);
        }

        Move(moveList);
    }

    private void Move(List<Vector3> moveList)
    {
        if (moveList.Count > 0)
        {
            Vector3 direction = moveList[0] - transform.position;
            direction = direction.normalized;

            transform.position += moveSpeed * direction * Time.deltaTime;
            if (Vector3.Distance(transform.position, moveList[0]) < 0.05f)
            {
                moveList.RemoveAt(0);
            }
        }
    }

    private void Move(Vector3 targetPos)
    {
        Vector3 direction = targetPos - transform.position;
        direction = direction.normalized;

        transform.position += moveSpeed * direction * Time.deltaTime;
    }
}
