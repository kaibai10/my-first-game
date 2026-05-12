using UnityEngine;

[CreateAssetMenu(menuName = "RandomEvents/EventActions/SetGameResource")]
public class ActionSetGameResource : EventActionBase
{
    public ResourcePack resources;

    public override void OnActive() => SetGameResource(resources);

    private void SetGameResource(ResourcePack resources) 
    {
        GameResourceManager.instance.SetCoin(resources.addCoin);
        GameResourceManager.instance.SetFood(resources.addFood);
        GameResourceManager.instance.SetMedicalItems(resources.addMedicalItems);
        GameResourceManager.instance.SetMineral(resources.addMineral);
        GameResourceManager.instance.SetPurifyEnergy(resources.addPurifyEnergy);
    }
}

[System.Serializable]
public struct ResourcePack 
{
    [Header("增加金币的数量")] public int addCoin;
    [Header("增加的食物数量")] public int addFood;
    [Header("增加的医疗品数量")] public int addMedicalItems;
    [Header("增加的能源矿数量")] public int addMineral;
    [Header("增加的净化能量数")] public int addPurifyEnergy;
}