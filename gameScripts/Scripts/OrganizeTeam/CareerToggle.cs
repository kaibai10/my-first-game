using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using TMPro;

public class CareerToggle : MonoBehaviour
{
    public TMP_Text careerNameText;
    public string assignedCareer;

    public void UpdateCareer(string theCareer)
    {
        careerNameText.text = theCareer;
        assignedCareer = theCareer;
    }

    public void ActivedToggle(bool isOn)
    {
        if (isOn)
        {
            Debug.Log("点击careerToggle");
            CharacterSelectSystem.instance.lastCareer = CharacterSelectSystem.instance.currentCareer;
            CharacterSelectSystem.instance.currentCareer = assignedCareer;
            CharacterSelectSystem.instance.UpdateCareerList();

            //更新技能栏

        }
    }
}
