using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SimpleCharacterState : MonoBehaviour
{
    public static SimpleCharacterState instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject father;
    public GameObject prafab;
    public TMP_Text nameText;
    public Slider healthSlider;

    public void CreateButton(LeaderController leader) 
    {
        GameObject newButton = Instantiate(prafab, father.transform);
        UpdataCreateButton(leader);
    }

    void UpdataCreateButton(LeaderController leader) 
    {
        nameText.text = leader.name;
        healthSlider.maxValue = leader.maxHealth;
        healthSlider.value = leader.currentHealth;
    }
}
