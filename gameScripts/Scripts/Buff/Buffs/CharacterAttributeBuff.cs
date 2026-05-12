using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/CharacterAttributebuff")]
public class CharacterAttributeBuff : BuffBase
{
    public CharacterAttribute characterAttribute;

    public override void ApplyBuff(GameObject self, GameObject target)
    {
        Debug.Log("给角色施加修改属性buff");
        AllUnitAttributeModify modify = target.GetComponent<AllUnitAttributeModify>();
        modify.ApplyBuffModify(characterAttribute);

        release = self;
    }

    public override void RemoveBuff(GameObject self, GameObject target)
    {
        Debug.Log("移除角色施加修改属性buff");
        AllUnitAttributeModify modify = target.GetComponent<AllUnitAttributeModify>();
        modify.RemoveBuffModify(characterAttribute);
    }
}

//为了让属性修改buff适用于所有角色（友方/敌方），让他们实现同一个接口
public interface AllUnitAttributeModify 
{
    /// <summary>
    /// 应用buff修改
    /// </summary>
    /// <param name="characterAttribute"></param>   
    void ApplyBuffModify(CharacterAttribute characterAttribute);

    /// <summary>
    /// 撤销buff修改
    /// </summary>
    /// <param name="characterAttribute"></param>
    void RemoveBuffModify(CharacterAttribute characterAttribute);
}