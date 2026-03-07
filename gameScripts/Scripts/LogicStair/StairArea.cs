using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairArea : MonoBehaviour
{
    [Tooltip("逻辑楼梯高层高度")] public int upperHeight;
    [Tooltip("逻辑楼梯低层高度")] public int lowerHeight;

    public void HandleChildTrigger(Collider2D collision, GameObject child)
    {
        Debug.Log("触发器为：" + collision.name);
        Debug.Log("孩子为：" + child.name);
        if (child.name == "_upper entrance")
        {
            Debug.Log(collision.name + "的层高为:" + collision.gameObject.layer);
            collision.gameObject.GetComponent<Height2D>().SetHeight(upperHeight);
            Debug.Log(collision.name + "的层高为:" + collision.gameObject.layer);
        }
        else if (child.name == "_lower entrance") 
        {
            Debug.Log(collision.name + "的层高为:" + collision.gameObject.layer);
            collision.gameObject.GetComponent<Height2D>().SetHeight(lowerHeight);
            Debug.Log(collision.name + "的层高为:" + collision.gameObject.layer);
        }
    }
}
