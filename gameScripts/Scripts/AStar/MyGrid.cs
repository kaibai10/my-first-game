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
                    TileBase tile = maps[h].GetTile(cellPos);
                    if (tile == null) continue;     //若该单元格不存在瓦片则跳过
                    Vector2 pos = maps[h].GetCellCenterWorld(cellPos);          //获取单元格在世界中的位置
                    
                    Tile.ColliderType cellType = maps[h].GetColliderType(cellPos);
                    bool walkable = cellType == Tile.ColliderType.None; //判断该位置是否可走 

                    MyNode node = new MyNode(cellPos.x, cellPos.y, h, pos, walkable);
                    nodes[i, j, h] = node;
                }
            }
        }
    }


    //把世界坐标（比如鼠标点击位置、角色当前位置等）转换成nodes数组的索引 (x,y,z)
    public List<Vector3Int> WorldToGridIndex(Vector3 worldPos)
    {
        Vector3Int cell = maps[0].WorldToCell(worldPos);
        List<Vector3Int> canMove = new List<Vector3Int>();

        for (int i = 0; i < mapHeight; i++) 
        {
            Vector3Int p = new Vector3Int(cell.x - minCell.x, cell.y - minCell.y, i);
            if (nodes[p.x, p.y, p.z] !=null)
                canMove.Add(p);
        }
        return canMove;
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

    public MyNode(int x, int y, int h, Vector2 pos, bool walkable)
    {
        this.x = x;
        this.y = y;     //Tilemap网格中的位置
        this.h = h;
        this.pos = pos; //世界中的位置
        this.walkable = walkable;
    }
}
