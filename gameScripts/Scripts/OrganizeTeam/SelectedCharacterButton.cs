using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedCharacterButton : MonoBehaviour
{
    public int index;
    public Image illillustration;
    public GameObject teamSelectPanel;

    public void UpdateButton() 
    {
        if (SelectedCharacter.instance.leaderControllers[index] != null)
        {
            illillustration.sprite = SelectedCharacter.instance.leaderControllers[index].illustration;
        }
        else 
        {
            Debug.Log("°´Å¥ÔªËØ" + index + "Îª¿Õ");
        }
    }

    public void ActiveButton() 
    {
        teamSelectPanel.gameObject.SetActive(true);
        SelectedCharacter.instance.currentMakingCharacterIndex = index;
        SelectedCharacter.instance.gameObject.SetActive(false);
    }
}
