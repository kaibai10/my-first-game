using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MyGrid : MonoBehaviour
{
    public static MyGrid instance;

    //TilpMap组件
    public List<Tilemap> maps;

    //A*寻路
    //public MyAStar aStar;
    public int Width;
    public int Height;

    //地图上所有的格子
    public MyNode[,,] nodes;

    private Vector3Int minCell;           // 地图左下角的格子坐标（真实世界坐标系）
    [Header("地图层高")]
    public int mapHeight;

    private void Awake()
    {
        instance = this;

        //盒体宽高
        Width = maps[0].cellBounds.size.x;
        Height = maps[0].cellBounds.size.y;
        nodes = new MyNode[Width, Height, mapHeight];
        minCell = maps[0].cellBounds.min;

        BuildGrid();
    }

    private void BuildGrid() 
    {
        for (int h = 0; h < mapHeight; h++)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Vector3Int cellPos = minCell + new Vector3Int(i, j, 0);     //单元格在Tilemap中的位置
                    Vector2 pos = maps[h].GetCellCenterWorld(cellPos);              //单元格的世界逻辑位置
                    Tile.ColliderType type = maps[h].GetColliderType(cellPos);
                    TileBase tile = maps[h].GetTile(cellPos);
                    bool walkable = tile != null && type == Tile.ColliderType.None;

                    MyNode node = new MyNode(i, j, h, pos, walkable);
                    for (int t = h; t < mapHeight; t++) //让底层单元格覆盖到每一层高层单元格
                    {
                        nodes[i, j, h] = node;
                    }
                }
            }
        }
    }

    //把世界坐标（比如鼠标点击位置、角色当前位置等）转换成nodes数组的索引 (x,y)
    public Vector3Int WorldToGridIndex(Vector3 worldPos)    //逻辑复杂，需要修改
    {
        int h = 0;
        for (int i = 0; i < mapHeight - 1; i++) 
        {
            Vector3Int c = maps[i].WorldToCell(worldPos);
            Tile.ColliderType type = maps[i].GetColliderType(c);
            TileBase tile = maps[i].GetTile(c);
            bool walkable = tile != null && type == Tile.ColliderType.None;
            if (!walkable) break;
            h = i;
        }

        Vector3Int cell = maps[h].WorldToCell(worldPos);
        return new Vector3Int(cell.x - minCell.x, cell.y - minCell.y, h);
    }
}

public class MyGridIndex
{
    public int x;
    public int y;
    public int h;
    public MyGridIndex(int x, int y, int h) { this.x = x; this.y = y; this.h = h; }
}

public class MyNode
{
    public int x;
    public int y;
    public int h;
    public Vector2 pos;
    public bool walkable;

    //A*使用参数
    public float gCost = 0;                 // 从起点到此的总代价
    public float hCost = 0;                 // 到终点的启发代价（曼哈顿/欧氏）
    public float fCost => gCost + hCost;    // 总代价
    public MyNode cameFrom;                 //到达该节点的上一个节点

    public MyNode(int x, int y, int h, Vector2 pos, bool walkable)
    {
        this.x = x;
        this.y = y;
        this.h = h;
        this.pos = pos;
        this.walkable = walkable;
    }
}