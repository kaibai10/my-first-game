using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttributeController : MonoBehaviour
{
    public static AttributeController instance;

    private void Awake()
    {
        instance = this;
    }

    public TMP_Text characterNameText, careerText, healthText, damageText, armorResistanceText, magicResistanceText;
    public TMP_Text weaponNameText, weaponLevelText, weapondamageText, weaponPenetrationText, criticalRateText, criticalDamageText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdataCharacter_Attribute(Character theCharacter)
    {
        characterNameText.text = theCharacter.name;
        careerText.text = theCharacter.career;
        healthText.text = theCharacter.currentHealth.ToString() + "/" + theCharacter.maxHealth.ToString();
        damageText.text = theCharacter.attackDamage.ToString();
        armorResistanceText.text = theCharacter.armorResistance.ToString();
        magicResistanceText.text = theCharacter.magicResistance.ToString();
    }
}
