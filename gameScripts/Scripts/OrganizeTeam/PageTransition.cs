using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageTransition : MonoBehaviour
{
    public static PageTransition instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject page1;
    //角色属性/技能面板的transform
    public GameObject detailedAttributes;
    public GameObject personalProfile;
    public GameObject characterSkillBox;
    public GameObject skillSelectionBox;

    public float alphaTransformSpeed;
    public float posTransformSpeed;


    public AnimationCurve showCurve;
    public AnimationCurve hideCurve;//曲线

    public bool isNextPage;

    //某对象可以移动的范围
    private float detailedAttributesMinXPos;
    private float personalProfileMinXPos;
    private float menusMinXPos;
    private float characterIllustrationMinXPos;

    private void Start()
    {
        detailedAttributesMinXPos = detailedAttributes.GetComponent<RectTransform>().anchoredPosition.x;
        personalProfileMinXPos = personalProfile.GetComponent<RectTransform>().anchoredPosition.x;
        menusMinXPos = careerMenu.GetComponent<RectTransform>().anchoredPosition.x - 172;
        characterIllustrationMinXPos = characterIllustration.GetComponent<RectTransform>().anchoredPosition.x - 172;
        
        //detailedAttributesMinXPos = detailedAttributes.transform.position.x;
        //personalProfileMinXPos = personalProfile.transform.position.x;
        //menusMinXPos = careerMenu.transform.position.x - 172;
        //characterIllustrationMinXPos = characterIllustration.transform.position.x - 172;
    }

    public void GoToNextPage() 
    {
        if (isNextPage == false)
        {
            isNextPage = true;
            StopAllCoroutines();
            //属性/技能
            StartCoroutine(ShowGameObject_(characterSkillBox, detailedAttributesMinXPos));
            StartCoroutine(ShowGameObject_(skillSelectionBox, personalProfileMinXPos));
            StartCoroutine(HideGameObject_(detailedAttributes, detailedAttributesMinXPos));
            StartCoroutine(HideGameObject_(personalProfile, personalProfileMinXPos));

            //职业/角色
            StartCoroutine(HideMenus_(careerMenu, menusMinXPos));
            StartCoroutine(HideMenus_(characterMenu, menusMinXPos));

            //角色立绘
            StartCoroutine(HideCharacterIllustration_(characterIllustration, characterIllustrationMinXPos));
        }
        else 
        {
            isNextPage = false;
            StopAllCoroutines();
            //属性/技能
            StartCoroutine(ShowGameObject_(detailedAttributes, detailedAttributesMinXPos));
            StartCoroutine(ShowGameObject_(personalProfile, personalProfileMinXPos));
            StartCoroutine(HideGameObject_(characterSkillBox, detailedAttributesMinXPos));
            StartCoroutine(HideGameObject_(skillSelectionBox, personalProfileMinXPos));

            //职业/角色
            StartCoroutine(ShowMenus_(careerMenu, menusMinXPos));
            StartCoroutine(ShowMenus_(characterMenu, menusMinXPos));

            //角色立绘
            StartCoroutine(ShowCharacterIllustration_(characterIllustration, characterIllustrationMinXPos));
        }
    }

    //角色属性/技能面板的transform
    IEnumerator ShowGameObject_(GameObject theGameobject, float minXPos)
    {
        theGameobject.SetActive(true);
        CanvasGroup canvasGroup = theGameobject.GetComponent<CanvasGroup>();
        float timer = 0;

        RectTransform rt= theGameobject.GetComponent<RectTransform>();
        Vector2 startPos = rt.anchoredPosition;
        Vector2 targetPos = new Vector2(minXPos, 0f);

        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha = showCurve.Evaluate(timer);
            timer += Time.deltaTime;
            startPos = Vector2.Lerp(startPos, targetPos, 0.015f);
            startPos.x = Mathf.Clamp(startPos.x, minXPos, minXPos + 50);
            rt.anchoredPosition = startPos;
            yield return null;
        }
        rt.anchoredPosition = targetPos;
    }

    IEnumerator HideGameObject_(GameObject theGameobject, float minXPos)
    {
        CanvasGroup canvasGroup = theGameobject.GetComponent<CanvasGroup>();
        float timer = 0;

        RectTransform rt = theGameobject.GetComponent<RectTransform>();
        Vector2 startPos = rt.anchoredPosition;
        Vector2 targetPos = new Vector2(minXPos + 50f, 0f);

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha = hideCurve.Evaluate(timer);
            timer += Time.deltaTime;
            startPos = Vector2.Lerp(startPos, targetPos, 0.015f);
            startPos.x = Mathf.Clamp(startPos.x, minXPos, minXPos + 50);
            rt.anchoredPosition = startPos;
            yield return null;
        }
        rt.anchoredPosition = targetPos;

        theGameobject.SetActive(false);
    }


    //职业/角色滚动视图的transform
    public GameObject careerMenu;
    public GameObject characterMenu;
    public float menusMoveSpeed;

    IEnumerator ShowMenus_(GameObject theGameobject, float minXPos)
    {
        theGameobject.SetActive(true);
        CanvasGroup canvasGroup = theGameobject.GetComponent<CanvasGroup>();
        float timer = 0;

        RectTransform rt = theGameobject.GetComponent<RectTransform>();
        Vector2 startPos = rt.anchoredPosition;
        Vector2 targetPos = new Vector2(minXPos + 172f, rt.anchoredPosition.y);

        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha = showCurve.Evaluate(timer);
            timer += Time.deltaTime;
            startPos = Vector2.Lerp(startPos, targetPos, 0.015f);
            startPos.x = Mathf.Clamp(startPos.x, minXPos, minXPos + 172);
            rt.anchoredPosition = startPos;
            yield return null;
        }

        rt.anchoredPosition = targetPos;
    }

    IEnumerator HideMenus_(GameObject theGameobject, float minXPos)
    {
        CanvasGroup canvasGroup = theGameobject.GetComponent<CanvasGroup>();
        float timer = 0;

        RectTransform rt = theGameobject.GetComponent<RectTransform>();
        Vector2 startPos = rt.anchoredPosition;
        Vector2 targetPos = new Vector2(minXPos, rt.anchoredPosition.y);

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha = hideCurve.Evaluate(timer);
            timer += Time.deltaTime;
            startPos = Vector2.Lerp(startPos, targetPos, 0.015f);
            startPos.x = Mathf.Clamp(startPos.x, minXPos, minXPos + 172);
            rt.anchoredPosition = startPos;
            yield return null;
        }

        rt.anchoredPosition = targetPos;
        theGameobject.SetActive(false);
    }


    //角色立绘Transform
    public GameObject characterIllustration;

    IEnumerator ShowCharacterIllustration_(GameObject theGameobject, float minXPos)
    {
        RectTransform rt = theGameobject.GetComponent <RectTransform>();
        Vector2 startPos = rt.anchoredPosition;
        Vector2 targetPos = new Vector2(minXPos + 172f, 0f);

        while (startPos.x < minXPos + 172) 
        {
            startPos = Vector2.Lerp(startPos, targetPos, 0.015f);
            startPos.x = Mathf.Clamp(startPos.x, minXPos, minXPos + 172);
            rt.anchoredPosition = startPos;
            yield return null;
        }
        rt.anchoredPosition = targetPos;
    }

    IEnumerator HideCharacterIllustration_(GameObject theGameobject, float minXPos)
    {
        RectTransform rt = theGameobject.GetComponent<RectTransform>();
        Vector2 startPos = rt.anchoredPosition;
        Vector2 targetPos = new Vector2(minXPos, 0f);

        while (startPos.x > minXPos)
        {
            startPos = Vector2.Lerp(startPos, targetPos, 0.015f);
            startPos.x = Mathf.Clamp(startPos.x, minXPos, minXPos + 172);
            rt.anchoredPosition = startPos;
            yield return null;
        }
        rt.anchoredPosition = targetPos;
    }
}
