using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class TeamSelectPanel : MonoBehaviour
{
    private ScrollRect scrollrect;
    //存储物体的容器
    public Transform contentTrans;

    //存储选中角色信息的容器
    [SerializeField]
    private List<LeaderController> leaders;

    //需要排列的物体
    [SerializeField]
    private List<Transform> contentsPos = new List<Transform>();

    //物体的数量
    private int contentCount;

    public float noSelectSpacing;
    public float selectSpacing;

    //检测移动的阈值
    public float repositionThreshold;
    public bool left;

    private void Start()
    {
        scrollrect = gameObject.GetComponent<ScrollRect>();
        contentCount = contentTrans.childCount;

        for (int i = 0; i < contentCount; i++) 
        {
            contentsPos.Add(contentTrans.GetChild(i));
        }
    }

    private void Update()
    {
        CheckAndRepositionItems();

        //根据物体的x值来对物体进行排序
        contentsPos.Sort((a,b)=> a.position.x.CompareTo(b.position.x));
        for (int i = 0; i < contentCount; i++) 
        {
            contentsPos[i].SetSiblingIndex(i);
        }
    }

    //检查并循环移动超出边界的卡片
    private void CheckAndRepositionItems() 
    {
        if (contentsPos[0].position.x - transform.position.x < -repositionThreshold)//向左
        {
            contentsPos[0].position = contentsPos[contentCount - 1].position + new Vector3(400f, 0f, 0f);
            contentsPos.Sort((a, b) => a.position.x.CompareTo(b.position.x));
            for (int i = 0; i < contentCount; i++)
            {
                contentsPos[i].SetSiblingIndex(i);
            }
            scrollrect.content.anchoredPosition3D += new Vector3(700f, 0f, 0f);
            for(int i = 0;i<contentCount;i++)
                contentsPos[0].position -= new Vector3(700f, 0f, 0f);
            Debug.Log("移动到右侧");
        }
        //else if (contentsPos[contentCount - 1].position.x - transform.position.x > repositionThreshold)//向右
        //{
        //    contentsPos[contentCount - 1].position = contentsPos[0].position - new Vector3(400f, 0f, 0f);
        //    scrollrect.content.anchoredPosition3D -= new Vector3(400f, 0f, 0f);
        //    Debug.Log("移动到左侧");
        //}
    }
}
