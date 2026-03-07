using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogWindowController : MonoBehaviour
{
    public static DialogWindowController instance;

    private void Awake()
    {
        instance = this;
    }

    public Image dialogueBack;
    public TMP_Text dialogueText;

    public AnimationCurve showCurve;
    public AnimationCurve hideCurve;
    public float animationSpeed;


    public void ShowDialogue() 
    {
        StopAllCoroutines();
        StartCoroutine(ShowDialogue_());
    }

    public void HideDialogue() 
    {
        StopAllCoroutines();
        StartCoroutine(HideDialogue_());
    }

    IEnumerator ShowDialogue_()
    {
        float timer = 0;
        while (dialogueBack.color.a < 1)
        {
            dialogueBack.color = new Vector4(1, 1, 1, showCurve.Evaluate(timer));
            dialogueText.color = new Vector4(0, 0, 0, showCurve.Evaluate(timer));
            timer += Time.deltaTime * animationSpeed;

            yield return null;
        }
    }
    IEnumerator HideDialogue_()
    {
        float timer = 0;
        while (dialogueBack.color.a > 0)
        {
            dialogueBack.color = new Vector4(1, 1, 1, hideCurve.Evaluate(timer));
            dialogueText.color = new Vector4(0, 0, 0, hideCurve.Evaluate(timer));
            timer += Time.deltaTime * animationSpeed;

            yield return null;
        }
    }

}
