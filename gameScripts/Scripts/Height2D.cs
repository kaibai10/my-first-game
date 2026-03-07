using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//游戏中各自角色的高度控制组件
public class Height2D : MonoBehaviour
{
    public Dictionary<int, int> layers = new Dictionary<int, int>();

    private void Start()
    {
        //角色层高和地图图层映射
        layers.Add(0, 21);      //CharacterHeight-0
        layers.Add(1, 22);      //CharacterHeight-1
        layers.Add(2, 23);      //CharacterHeight-2

        SetHeight(0);
    }

    public void SetHeight(int height) 
    {
        gameObject.layer = layers[height];
    }
}
