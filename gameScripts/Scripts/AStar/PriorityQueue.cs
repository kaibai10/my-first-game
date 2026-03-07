using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

//使用于A*的特化优先队列，采用小根堆排序
public class PriorityQueue
{
    private List<Node> nodeList = new List<Node>();

    //堆节点
    private class Node
    {
        //数据
        public object data { get; set; }

        //优先队列的比较值
        public float val { get; set; }

        public Node(object data, float val)
        {
            this.data = data;
            this.val = val;
        }
    }

    //初始化优先队列，序列0处不使用，用null占用
    public PriorityQueue() 
    {
        nodeList.Add(null);
    }

    //获取队列数据数量
    public int GetCount() 
    {
        return nodeList.Count - 1;
    }

    //添加数据
    public void Push(object data, float val) 
    {
        nodeList.Add(new Node(data, val));

        Up(nodeList.Count - 1);
    }

    //取出数据
    public object Pop() 
    {
        if (nodeList.Count <= 1) return null;

        Node node = nodeList[1];
        nodeList[1] = nodeList[nodeList.Count - 1];
        nodeList.RemoveAt(nodeList.Count - 1);
        Down(1);

        return node.data;
    }

    //上浮
    private void Up(int addIndex)
    {
        if (addIndex > 1 && nodeList[addIndex / 2].val > nodeList[addIndex].val) 
        {
            Node node = nodeList[addIndex / 2];
            nodeList[addIndex / 2] = nodeList[addIndex];
            nodeList[addIndex] = node;

            //递归，直到所有堆完全有序
            Up(addIndex / 2);
        }
    }

    //下沉
    private void Down(int index)
    {
        //与当前点位作比较的下一个点的下标
        int targetIndex = 0;

        // 左孩子是否存在
        if (index * 2 < nodeList.Count)
            targetIndex = index * 2;
        else
            return;

        //右孩子是否存在，如果存在与左孩子进行比较，取较小的一个
        if (targetIndex + 1 < nodeList.Count && nodeList[targetIndex].val > nodeList[targetIndex + 1].val)
            targetIndex++;

        //与孩子进行比较
        if (nodeList[index].val < nodeList[targetIndex].val)
            return;

        Node node = nodeList[index];
        nodeList[index] = nodeList[targetIndex];
        nodeList[targetIndex] = node;

        //递归，直到完全有序
        Down(targetIndex);
    }
}
