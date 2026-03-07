using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CareerSelectMenu : MonoBehaviour
{
    public TMP_Text currentCareerName;
    public List<CareerToggle> careerToggleGroup;
    public Button showCarreers;
    public Image arrow_left;
    public CanvasGroup careersGroup;
    public float speed;
    private bool isShowCareer;

    public GameObject careerView;   //职业选择视图

    public AnimationCurve showCurve;
    public AnimationCurve hideCurve;//曲线

    //public List<CharacterToggle> characterToggleGroup;
    public Image arrow_down;
    public CanvasGroup characterGroup;
    private bool isShowCharacter;

    public GameObject characterView;//角色选择视图

    private float careerViewMinXPos;
    private float characterViewMinYPos;

    // Update is called once per frame
    void Start()
    {
        careerViewMinXPos = careerView.GetComponent<RectTransform>().anchoredPosition.x;
        characterViewMinYPos = characterView.GetComponent<RectTransform>().anchoredPosition.y - 160f;
    }

    public void ShowAllCareer() 
    {
        if (isShowCareer == false)
        {
            isShowCareer = true;
            StopAllCoroutines();
            StartCoroutine(ShowCareer_(careerViewMinXPos));
        }
        else 
        {
            isShowCareer = false;
            StopAllCoroutines();
            StartCoroutine(HideCareer_(careerViewMinXPos));
        }
    }

    IEnumerator ShowCareer_(float minXPos)
    {
        careerView.SetActive(true);

        RectTransform rt = careerView.GetComponent<RectTransform>();
        Vector2 startPos = rt.anchoredPosition;
        Vector2 targetPos = new Vector2(minXPos + 190f, 0f);
        float timer = 0;

        while (careersGroup.alpha < 1)
        {
            careersGroup.alpha = showCurve.Evaluate(timer);
            arrow_left.transform.rotation = Quaternion.Euler(new Vector3(0, showCurve.Evaluate(timer) * 180, 90));
            timer += Time.deltaTime;

            startPos = Vector2.Lerp(startPos, targetPos, 0.015f);
            startPos.x = Mathf.Clamp(startPos.x, minXPos, minXPos + 190);
            rt.anchoredPosition = startPos;
            yield return null;
        }

        rt.anchoredPosition = targetPos;
    }

    IEnumerator HideCareer_(float minXPos)
    {
        RectTransform rt = careerView.GetComponent<RectTransform>();
        Vector2 startPos = rt.anchoredPosition;
        Vector2 targetPos = new Vector2(minXPos, 0f);
        float timer = 0;

        while (careersGroup.alpha > 0)
        {
            careersGroup.alpha = hideCurve.Evaluate(timer);
            arrow_left.transform.rotation = Quaternion.Euler(new Vector3(0, hideCurve.Evaluate(timer) * 180, -90));
            timer += Time.deltaTime;

            startPos = Vector2.Lerp(startPos, targetPos, 0.015f);
            startPos.x = Mathf.Clamp(startPos.x, minXPos, minXPos + 190);
            rt.anchoredPosition = startPos;
            yield return null;
        }

        rt.anchoredPosition = targetPos;
        careerView.SetActive(false);

        //float timer = 0;

        //while (careersGroup.alpha > 0)
        //{
        //    careersGroup.alpha = hideCurve.Evaluate(timer);
        //    arrow_left.transform.rotation = Quaternion.Euler(new Vector3(0, hideCurve.Evaluate(timer) * 180, -90));
        //    timer += Time.deltaTime; 
        //    careerView.transform.position -= Vector3.right * speed * Time.deltaTime;
        //    yield return null;
        //}
    }

    public void ShowAllCharacter() 
    {
        if (isShowCharacter == false)
        {
            isShowCharacter = true;
            StopAllCoroutines();
            StartCoroutine(ShowCharacter_(characterViewMinYPos));
        }
        else
        {
            isShowCharacter = false;
            StopAllCoroutines();
            StartCoroutine(HideCharacter_(characterViewMinYPos));
        }
    }

    IEnumerator ShowCharacter_(float minYPos)
    {
        characterView.SetActive(true);

        RectTransform rt = characterView.GetComponent<RectTransform>();
        Vector2 startPos = rt.anchoredPosition;
        Vector2 targetPos = new Vector2(0f, minYPos);
        float timer = 0;

        while (characterGroup.alpha < 1)
        {
            characterGroup.alpha = showCurve.Evaluate(timer);
            arrow_down.transform.rotation = Quaternion.Euler(new Vector3(showCurve.Evaluate(timer) * 180, 0, 0));
            timer += Time.deltaTime;
            startPos = Vector2.Lerp(startPos, targetPos, 0.015f);
            startPos.y = Mathf.Clamp(startPos.y, minYPos, minYPos + 160);
            rt.anchoredPosition = startPos;
            yield return null;
        }
        rt.anchoredPosition = targetPos;
    }

    IEnumerator HideCharacter_(float minYPos)
    {
        RectTransform rt = characterView.GetComponent<RectTransform>();
        Vector2 startPos = rt.anchoredPosition;
        Vector2 targetPos = new Vector2(0f, minYPos + 160f);
        float timer = 0;

        while (characterGroup.alpha > 0)
        {
            characterGroup.alpha = hideCurve.Evaluate(timer);
            arrow_down.transform.rotation = Quaternion.Euler(new Vector3(hideCurve.Evaluate(timer) * 180, 0, 0));
            timer += Time.deltaTime;
            startPos = Vector2.Lerp(startPos, targetPos, 0.015f);
            startPos.y = Mathf.Clamp(startPos.y, minYPos, minYPos + 160);
            rt.anchoredPosition = startPos;
            yield return null;
        }
        rt.anchoredPosition = targetPos;
        characterView.SetActive(false);

        //float timer = 0;

        //while (characterGroup.alpha > 0)
        //{
        //    characterGroup.alpha = hideCurve.Evaluate(timer);
        //    arrow_down.transform.rotation = Quaternion.Euler(new Vector3(hideCurve.Evaluate(timer) * 180, 0, 0));
        //    timer += Time.deltaTime;
        //    characterView.transform.position += Vector3.up * speed * Time.deltaTime;
        //    yield return null;
        //}
    }
}
