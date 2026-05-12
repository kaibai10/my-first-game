using UnityEngine;

public abstract class BuffBase : ScriptableObject
{
    [Header("Buff基本信息")]
    public string buffName;
    public Sprite icon;
    public float duration;  //持续时间
    [TextArea] public string buffDescription;   //buff效果描述

    [Header("叠加设置")]
    public string buffTypeID;   //用于分组的ID
    public BuffStackMode stackMode;
    public float powerLevel;        //强度数值（用于 HighestValue 模式的比较）

    [HideInInspector] public GameObject release;

    /// <summary>
    /// buff应用效果
    /// </summary>
    /// <param name="self"> self指技能释放者 </param>
    /// <param name="target"> target指技能目标 </param>  
    public abstract void ApplyBuff(GameObject self, GameObject target);

    /// <summary>
    /// buff移除效果
    /// </summary>
    /// <param name="self"> self指技能释放者 </param>
    /// <param name="target"> target指技能目标 </param>
    public abstract void RemoveBuff(GameObject self, GameObject target);
}

// Buff叠加规则
public enum BuffStackMode
{
    Stackable,      // 允许重叠：每个 Buff 独立存在，效果叠加
    HighestValue,     // 取最大值：同组 Buff 中只保留效果最强的一个
    //RefreshDuration, // 仅刷新时间：同种 Buff 只有一个，新来的刷新倒计时
}