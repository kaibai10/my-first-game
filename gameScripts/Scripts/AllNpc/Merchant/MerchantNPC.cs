using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantNPC : NPCBase
{
    //商人在创建时需要更新自身的售卖物品列表
    public List<ItemsData> allItems;    // 所有可销售的物品
    public List<ItemsData> inventory;  //实际售出的商品列表
    private int sellCount;  //售卖的物品数量

    public override void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            eventTrigger();
        }
    }

    public override void eventTrigger()
    {
        //显示页面
        Debug.Log("商人：发送打开商店页面请求");
        GameEvents.TriggerShopRequest(inventory);
    }

    //随机生成销售物品
    private void SetSellItems() 
    {
        
    }
}
