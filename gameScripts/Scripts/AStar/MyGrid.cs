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

        foreach (var map in maps)
        {
            map.origin = maps[0].origin;  // 强制所有层用底层 origin
            map.CompressBounds();         // 压缩，但不改 origin
        }

        BuildGrid();
    }

    private void BuildGrid()
    {
        //导致地图大小很大的原因
        //// 先找到全局最小的 min（所有层的最左下）
        //Vector3Int globalMin = maps[0].cellBounds.min;
        //foreach (var map in maps)
        //{
        //    globalMin = Vector3Int.Min(globalMin, map.cellBounds.min);
        //}

        //// 然后用 globalMin 作为统一原点
        //minCell = globalMin;

        //// Width/Height 用全局最大范围
        //Vector3Int globalMax = maps[0].cellBounds.max;
        //foreach (var map in maps)
        //{
        //    globalMax = Vector3Int.Max(globalMax, map.cellBounds.max);
        //}
        //Width = globalMax.x - globalMin.x;
        //Height = globalMax.y - globalMin.y;

        nodes = new MyNode[Width, Height, mapHeight];

        for (int h = 0; h < mapHeight; h++)
        {
            var map = maps[h];
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Vector3Int localCell = minCell + new Vector3Int(i, j, 0);
                    Vector3 worldCenter = map.GetCellCenterWorld(localCell);
                    TileBase tile = map.GetTile(localCell);
                    Tile.ColliderType col = map.GetColliderType(localCell);

                    bool walkable = tile != null && col == Tile.ColliderType.None;

                    MyNode node = new MyNode(i, j, h, (Vector2)worldCenter, walkable);

                    Debug.Log($"单元格({i},{j},{h})初始化完成,其tile是否存在：{tile},其walkable为:{walkable}");
                    // 底层 node 向上层复制
                    if (tile == null) continue;
                    for (int t = h; t < mapHeight; t++)
                    {
                        nodes[i, j, t] = node;  // 注意：这里 t 从 h 开始，但通常想让低层覆盖高层空位
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