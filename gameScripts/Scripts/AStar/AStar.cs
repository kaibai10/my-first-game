using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    //path[]存储路径

    public List<MyNode> FindPath(MyGridIndex startGrid, MyGridIndex endGrid)
    {
        Dictionary<MyNode, MyNode> cameFrom = new Dictionary<MyNode, MyNode>();     //到达该节点的上一个节点
        Dictionary<MyNode, float> cost = new Dictionary<MyNode, float>();           //初始节点到当前节点的代价

        //优先队列
        PriorityQueue priorityQueue = new PriorityQueue();

        MyNode startNode = MyGrid.instance.nodes[startGrid.x, startGrid.y];
        MyNode endNode = MyGrid.instance.nodes[endGrid.x, endGrid.y];

        cameFrom[startNode] = null;
        cost[startNode] = 0;

        priorityQueue.Push(startNode, 0);

        while (priorityQueue.GetCount() > 0)
        {
            MyNode front = (MyNode)priorityQueue.Pop();
            if (front == endNode) break;

            foreach (MyNode node in GetNeighbors(front))
            {
                float newGCost = cost[front] + GetDistance(front, node);
                if (!cost.ContainsKey(node) || cost[node] > newGCost)    //如果相邻节点没被检查过或发现了到该单元格花费更少的路径则更新
                {
                    cost[node] = newGCost;
                    float fCost = newGCost + heuristic(node, endNode);
                    priorityQueue.Push(node, fCost);            //遇到更好的节点时可能会重复插入，导致低效
                    cameFrom[node] = front;
                }
            }
        }

        return GetPath(startNode, endNode, cameFrom);
    }

    //启发式函数(曼哈顿距离)
    private float heuristic(MyNode currentNode, MyNode endNode)
    {
        return Mathf.Abs(currentNode.x - endNode.x) + Mathf.Abs(currentNode.y - endNode.y);
    }

    //寻找相邻节点（八方向）
    private List<MyNode> GetNeighbors(MyNode node)
    {
        List<MyNode> neighbors = new List<MyNode>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue; //跳过自身
                int nx = node.x + i;
                int ny = node.y + j;
                if (nx < 0 || ny < 0 || nx >= MyGrid.instance.Width || ny >= MyGrid.instance.Height) continue;    //坐标超出范围则跳过
                if (MyGrid.instance.nodes[nx, ny].walkable == false) continue;      //如果该单元格为碰撞体则跳过

                //对角检测：对角线移动时，必须同时保证两个相邻的直线方向也是可走的
                if (i == 0 || j == 0)//直线移动直接添加
                {
                    neighbors.Add(MyGrid.instance.nodes[nx, ny]);
                    continue;
                }

                bool canDiagonal = true;
                // 检查水平方向的中间格
                if (!MyGrid.instance.nodes[node.x + i, node.y].walkable)
                    canDiagonal = false;

                // 检查垂直方向的中间格
                if (!MyGrid.instance.nodes[node.x, node.y + j].walkable)
                    canDiagonal = false;

                if (canDiagonal)
                {
                    neighbors.Add(MyGrid.instance.nodes[nx, ny]);
                }
            }
        }
        return neighbors;
    }

    //相邻节点的距离（没有地形代价）
    private float GetDistance(MyNode currentNode, MyNode lastNode)
    {
        int dx = Mathf.Abs(currentNode.x - lastNode.x);
        int dy = Mathf.Abs(currentNode.y - lastNode.y);
        if (dx + dy == 1) return 1f;  // 直邻
        return 1.414f;                // 对角（√2）
    }

    private List<MyNode> GetPath(MyNode startNode, MyNode endNode, Dictionary<MyNode, MyNode> cameFrom)
    {
        if (!cameFrom.ContainsKey(endNode)) return null;  //若endNode不存在，则无路径

        List<MyNode> path = new List<MyNode>();
        path.Add(endNode);
        MyNode currentNode = endNode;

        while (currentNode != startNode)
        {
            currentNode = cameFrom[currentNode];
            path.Add(currentNode);
        }
        path.Reverse();
        return path;
    }
}
