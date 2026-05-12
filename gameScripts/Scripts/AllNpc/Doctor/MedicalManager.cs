using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicalManager : MonoBehaviour
{
    private void OnEnable()
    {
        GameEvents.OnTreatRequest += HandleTreatRequest;
    }

    private void OnDisable()
    {
        GameEvents.OnTreatRequest -= HandleTreatRequest;
    }


    public void HandleTreatRequest(LeaderController leader) 
    {
        int curCoinsCount = GameResourceManager.instance.GetCoin();
        int needCost = GetMedicalCost(leader);

        if (curCoinsCount > needCost)
        {
            Debug.Log("成员" + leader.characterName + "已恢复至健康状态");
            //具体恢复函数
            Treat(leader);
        }
        else 
        {
            //播放音效
            Debug.Log("金币不足");
        }
    }

    /// <summary>
    /// 具体的治疗逻辑
    /// </summary>
    /// <param name="leader"></param> 
    public void Treat(LeaderController leader) 
    { }

    /// <summary>
    /// 计算治疗所需的费用
    /// </summary>
    /// <param name="leader"></param>
    public int GetMedicalCost(LeaderController leader)
    {
        //待实现
        return 0;
    }
}
