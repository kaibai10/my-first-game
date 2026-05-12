using UnityEngine;

[CreateAssetMenu(menuName = "RandomEvents/EventActions/SetCharacterAttribute")]
public class ActionSetEnemiesAttribute : EventActionBase
{
    public CharacterAttribute enemyAttributes;

    public override void OnActive() => SetEnemiesAttribute(enemyAttributes);

    private void SetEnemiesAttribute(CharacterAttribute enmeyAttributes) 
    {
        
    }
}

[System.Serializable]
public struct CharacterAttribute 
{
    [Header("目标对象生命的增加量")] public float addHealth;
    [Header("目标对象攻击力的增加量")] public float addAttack;
    [Header("目标对象物理防御的增加量")] public float addArmorResistance;
    [Header("目标对象法术防御的增加量")] public float addMagicResistance;
}
