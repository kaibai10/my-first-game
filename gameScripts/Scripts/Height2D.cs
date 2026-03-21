using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

//游戏中各自角色的高度控制组件
public class Height2D : MonoBehaviour
{
    //角色图层
    private Dictionary<int, int> layers = new Dictionary<int, int>();
    //角色排序图层
    private Dictionary<int, string> sortLayers = new Dictionary<int, string>();


    private void Start()
    {
        //角色层高和地图图层映射
        layers.Add(0, 21);      //CharacterHeight-0
        layers.Add(1, 22);      //CharacterHeight-1
        layers.Add(2, 23);      //CharacterHeight-2

        //角色层高和排序图层映射
        sortLayers.Add(0, "CharacterHeight-0");
        sortLayers.Add(1, "CharacterHeight-1");
        sortLayers.Add(2, "CharacterHeight-2");

        SetHeight(0);
    }

    //设置角色所在高度层
    public void SetHeight(int height) 
    {
        gameObject.layer = layers[height];
    }

    //设置角色所在排序层
    public void SetSortLayer(int height)
    {
        gameObject.GetComponent<SortingGroup>().sortingLayerName = sortLayers[height];
    }
}
