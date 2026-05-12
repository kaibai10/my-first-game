using System;
using System.Collections.Generic;

public static class GameEvents
{
    // 定义一个要求打开商店的事件，参数是商店的数据
    // Action<T> 表示带有一个参数的委托

    //--------------------Merchant事件--------------------
    public static event Action<List<ItemsData>> OnShopRequest;
    // 购买物品请求
    public static event Action<ItemsData> OnPurchaseRequest;

    /// <summary>
    /// 触发打开商店委托
    /// </summary>
    /// <param name="itemsDatas"></param>
    public static void TriggerShopRequest(List<ItemsData> itemsDatas)
    {
        OnShopRequest?.Invoke(itemsDatas);
    }

    /// <summary>
    /// 触发购买委托
    /// </summary>
    /// <param name="itemData"></param>
    public static void TriggerPurchaseRequest(ItemsData itemData) 
    {
        OnPurchaseRequest?.Invoke(itemData);
    }

    //--------------------Doctor事件--------------------
    // 打开商店面板委托
    public static event Action OpenMedicalPanelRequest;
    //治疗委托
    public static event Action<LeaderController> OnTreatRequest;

    /// <summary>
    /// 触发打开商店面板委托
    /// </summary>
    public static void TriggerOpenMedicalPanelRequest() 
    {
        OpenMedicalPanelRequest?.Invoke();
    }

    /// <summary>
    /// 触发治疗请求
    /// </summary>
    /// <param name="leader"></param>
    public static void TriggerTreatRequest(LeaderController leader) 
    {
        OnTreatRequest?.Invoke(leader);
    }
}
