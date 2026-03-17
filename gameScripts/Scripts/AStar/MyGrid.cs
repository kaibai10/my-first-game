using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MyGrid : MonoBehaviour
{
    public static MyGrid instance;

    //TilpMap组件
    public Tilemap map;

    //A*寻路
    //public MyAStar aStar;
    public int Width;
    public int Height;
    //格子原点位置
    private Vector3 GridOrigionPos;
    //格子大小
    private float GridSize;

    //地图上所有的格子
    public MyNode[,] nodes;

    private Vector3Int minCell;           // 地图左下角的格子坐标（真实世界坐标系）

    private void Awake()
    {
        instance = this;

        //盒体宽高
        Width = map.cellBounds.size.x;
        Height = map.cellBounds.size.y;
        nodes = new MyNode[Width, Height];
        //单元格宽高（正方形）
        GridSize = map.cellSize.x;
        GridOrigionPos = map.GetCellCenterWorld(new Vector3Int(-Width / 2, -Height / 2));

        minCell = map.cellBounds.min;

        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                Vector3Int cellPos = minCell + new Vector3Int(i, j, 0);     //单元格在Tilemap中的位置
                Vector2 pos = map.GetCellCenterWorld(cellPos);              //单元格的世界逻辑位置
                Tile.ColliderType type = map.GetColliderType(cellPos);
                TileBase tile = map.GetTile(cellPos);
                bool walkable = tile != null && type == Tile.ColliderType.None;

                MyNode node = new MyNode(i, j, pos, walkable);
                nodes[i, j] = node;
            }
        }
    }


    //把世界坐标（比如鼠标点击位置、角色当前位置等）转换成nodes数组的索引 (x,y)
    public Vector2Int WorldToGridIndex(Vector3 worldPos)
    {
        Vector3Int cell = map.WorldToCell(worldPos);
        return new Vector2Int(cell.x - minCell.x, cell.y - minCell.y);
    }
}

public class MyGridIndex
{
    public int x;
    public int y;
    public MyGridIndex(int x, int y) { this.x = x; this.y = y; }
}

public class MyNode
{
    public int x;
    public int y;
    public Vector2 pos;
    public bool walkable;

    //A*使用参数
    public float gCost = 0;                 // 从起点到此的总代价
    public float hCost = 0;                 // 到终点的启发代价（曼哈顿/欧氏）
    public float fCost => gCost + hCost;    // 总代价
    public MyNode cameFrom;                 //到达该节点的上一个节点

    public MyNode(int x, int y, Vector2 pos, bool walkable)
    {
        this.x = x;
        this.y = y;
        this.pos = pos;
        this.walkable = walkable;
    }
}