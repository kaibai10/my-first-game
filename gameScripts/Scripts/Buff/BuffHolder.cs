using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuffHolder : MonoBehaviour
{
    private List<ActiveBuff> activeBuffs = new List<ActiveBuff>();

    private void Update()
    {
        // 更新buff的剩余时间
        for (int i = activeBuffs.Count - 1; i >= 0; i--) 
        {
            activeBuffs[i].remainingTime -= Time.deltaTime;
            if (activeBuffs[i].remainingTime <= 0) 
            {
                ActiveBuff curBuff = activeBuffs[i];
                //buff结束，触发撤销buff效果方法(只有当isApply==true时才撤销，否则会重复撤销)
                if (curBuff.isApply)
                    curBuff.buff.RemoveBuff(curBuff.creator, gameObject);
                activeBuffs.RemoveAt(i);

                //如果结束的技能类型为同种类最强，那么需要在此类型中重新选出一个最强的buff启用
                if (curBuff.buff.stackMode == BuffStackMode.HighestValue && curBuff.isApply)
                {
                    ActiveBuff bestBuff = FindBestBuff(curBuff);
                    if (bestBuff != null)
                    {
                        bestBuff.buff.ApplyBuff(bestBuff.creator, gameObject);
                        bestBuff.isApply = true;
                    }
                }
            }
        }
    }

    public void AddBuffBase(GameObject release, BuffBase newBuff) 
    {
        float newRemainingTime = newBuff.duration;
        switch (newBuff.stackMode) 
        {
            //如果buff为可叠加类型
            case BuffStackMode.Stackable:
                ActiveBuff newActiveBuff = new ActiveBuff(newBuff, newBuff.duration, release, true);
                activeBuffs.Add(newActiveBuff);
                newBuff.ApplyBuff(release, gameObject);
                break;

            //如果buff为同类型取最大值
            case BuffStackMode.HighestValue:
                HandleHighestValueBuff(release, newBuff);
                break;
        }
    }

    private void HandleHighestValueBuff(GameObject release, BuffBase newBuff) 
    {
        List<ActiveBuff> sameTypeBuffs = activeBuffs.FindAll(b=>b.buff.buffTypeID == newBuff.buffTypeID);    //按buff类型Id比较，在这里实际为buff的名字
        ActiveBuff newActiveBuff = new ActiveBuff(newBuff, newBuff.duration, release, false);  

        if (sameTypeBuffs.Count == 0) 
        {
            //直接添加并应用其ApplyBuff方法    
            activeBuffs.Add(newActiveBuff);
            newActiveBuff.isApply = true;
            newBuff.ApplyBuff(release, gameObject);
            return;
        }

        //检查是否有比当前新 Buff 更强的存在
        bool hasBetter = false;
        ActiveBuff bestbuff = FindBestBuff(newActiveBuff);
        if (bestbuff != null && bestbuff.buff.powerLevel >= newBuff.powerLevel) hasBetter = true;

        //如果场上有更强的buff，则之将其加入到activeBuffs列表中但不调用其ApplyBuff方法
        //如果没有更强的buff，则先撤销应用目前最强的buff效果，该为触发当前新的buff效果
        if (hasBetter) activeBuffs.Add(newActiveBuff);
        else 
        {
            bestbuff.buff.RemoveBuff(bestbuff.creator, gameObject);
            bestbuff.isApply = false;

            activeBuffs.Add(newActiveBuff);
            newActiveBuff.isApply = true;
            newBuff.ApplyBuff(release, gameObject);
        }
    }

    //寻找与参数同类型buff中最强的buff
    private ActiveBuff FindBestBuff(ActiveBuff targetBuff) 
    {
        List<ActiveBuff> sameTypeBuffs = activeBuffs.FindAll(b => b.buff.buffTypeID == targetBuff.buff.buffTypeID);
        if (sameTypeBuffs.Count == 0) return null;
        
        ActiveBuff bestBuff = sameTypeBuffs[0];

        foreach (var buff in sameTypeBuffs) 
        {
            if (bestBuff.buff.powerLevel < buff.buff.powerLevel) 
                bestBuff = buff;
        }
        return bestBuff;
    }

    //private void CreateNewBuff(GameObject release, BuffBase buffData)可替换case Stackable
    //{
    //    ActiveBuff newActiveBuff = new ActiveBuff(buffData, buffData.duration, release);
    //    activesBuffs.Add(newActiveBuff);
    //    buffData.ApplyBuff(release, gameObject);
    //}
}
