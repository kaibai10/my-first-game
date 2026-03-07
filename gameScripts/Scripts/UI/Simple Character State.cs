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

    public void CreateButton(Controllable theControllable) 
    {
        GameObject newButton = Instantiate(prafab, father.transform);
        UpdataCreateButton(theControllable);
    }

    void UpdataCreateButton(Controllable theControllable) 
    {
        nameText.text = theControllable.name;
        healthSlider.maxValue = theControllable.maxHealth;
        healthSlider.value = theControllable.currentHealth;
    }
}
