using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SideBarAnimator : MonoBehaviour , IPointerEnterHandler ,IPointerExitHandler
{
    [Header("引用组件")]
    public RectTransform contentRoot;
    public LayoutElement rootLayoutElement; //contentRoot上挂载的contentRoot
    public RectTransform optionButton;      //背包选项按钮
    public GameObject backpackPanel;        //背包面板
    public RectTransform backpackPanelRect; //背包面板Rect
    public Button closeButton;

    [Header("按钮触发函数")]
    public UnityEvent onClickActive;

    [Header("动画参数")]
    public float duration;  //动画时长
    public float buttonHoverScale;  //鼠标悬停时按钮的缩放幅度
    public float buttonHoverDist;   //鼠标悬停时按钮的移动距离
    public float panelActiveDist;   //触发按钮时面板的移动距离

    [Header("高度配置")]
    public float buttonHeight = 100f;  //按钮基础高度
    public float panelHeight = 330f;   //触发按钮后的面板高度（根据触发的按钮进行调整）

    private bool buttonIsAction = false; //按钮是否已经激活
    private Coroutine activeCoroutine;  //当前触发的协程

    void Start()
    {
        if(rootLayoutElement != null)
            rootLayoutElement.preferredHeight = buttonHeight;
        if(backpackPanel != null)
            backpackPanel.SetActive(false);
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseButton);
    }

    public void OnPointerEnter(PointerEventData eventData) 
    {
        if (buttonIsAction) return;  //如果按钮已经激活，则不作处理
        StartAnimation(buttonHoverScale, -buttonHoverDist, buttonHeight, false);
    }

    public void OnPointerExit(PointerEventData eventData) 
    {
        if (buttonIsAction) return;
        StartAnimation(1f, 35f, buttonHeight, false);
    }

    public void OnClickButton() 
    {
        //如果有面板则展开
        if (backpackPanel != null)
        {
            buttonIsAction = true;
            backpackPanel.SetActive(true);

            StartAnimation(1f, -panelActiveDist, panelHeight, true);
        }
        else 
        {
            onClickActive?.Invoke();
        }
    }

    public void CloseButton() 
    {
        if (backpackPanel != null) 
        {
            StartAnimation(1f, 35f, buttonHeight, false);
            //backpackPanel.SetActive(false);
            buttonIsAction = false;
        }
    }

    private void StartAnimation(float targetScale, float targetXPos, float targetHeight,bool panelIsShow) 
    {
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(Animation(targetScale, targetXPos, targetHeight, panelIsShow));
    }

    private IEnumerator Animation(float targetScale, float targetXPos, float targetHeight, bool panelIsShow) 
    {
        //按钮的属性
        Vector3 startScale = optionButton.localScale;
        float startXPos = optionButton.anchoredPosition.x;
        float startHeight = rootLayoutElement.preferredHeight;

        //按钮关联面板的属性
        float startPanelXPos = 0;
        if (backpackPanelRect != null) 
            startPanelXPos = backpackPanelRect.anchoredPosition.x;
        float targetPanelXPos = panelIsShow ? 0f : panelActiveDist;

        float elapsead = 0f;
        while (elapsead < duration) 
        {
            elapsead += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsead / duration);

            optionButton.localScale = Vector3.Lerp(startScale, new Vector3(targetScale, targetScale, 1f), t);
            
            float curXPos = Mathf.Lerp(startXPos, targetXPos, t);
            optionButton.anchoredPosition = new Vector2(curXPos, optionButton.anchoredPosition.y);

            if (backpackPanelRect != null) 
            {
                float curPanelXPos = Mathf.Lerp(startPanelXPos, targetPanelXPos, t);
                backpackPanelRect.anchoredPosition = new Vector2(curPanelXPos, backpackPanelRect.anchoredPosition.y);
            }

            rootLayoutElement.preferredHeight = Mathf.Lerp(startHeight, targetHeight, t);
            yield return null;
        }

        optionButton.localScale = new Vector3(targetScale, targetScale, 1f);
        optionButton.anchoredPosition = new Vector2(targetXPos, optionButton.anchoredPosition.y);
        backpackPanelRect.anchoredPosition = new Vector2(targetPanelXPos, backpackPanelRect.anchoredPosition.y);

        if (backpackPanel != null && !panelIsShow && backpackPanel.activeSelf == true)
            backpackPanel.SetActive(false);
    }
}
