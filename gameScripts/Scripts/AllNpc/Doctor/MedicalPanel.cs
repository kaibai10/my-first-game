using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MedicalPanel : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Button closeButton;

    public Transform contentFolder;
    public GameObject slotPrefab;

    public RectTransform rt;
    private Vector2 originalPosition; // 记录面板的初始位置(显示时的位置)
    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
        originalPosition = rt.anchoredPosition;
    }

    public void Show() 
    {
        gameObject.SetActive(true);
        UpdateMedicalInfo();

        StopAllCoroutines();
        StartCoroutine(FadeRoutine(1f, 0.2f, true));
    }

    public void Hide() 
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(0f, 0.2f, false));
    }

    //更新医疗面板上的角色信息
    private void UpdateMedicalInfo() 
    {
        Debug.Log("更新医疗面板信息");
        foreach (Transform child in contentFolder) 
        {
            Destroy(child.gameObject);
        }

        //需要一个持续维护场景中LeaderController对象的静态类
        foreach (var data in AllCharacter.leaderControllers) 
        {
            GameObject obj = Instantiate(slotPrefab, contentFolder);
            obj.SetActive(true);
            MedicalSlot slot = obj.GetComponent<MedicalSlot>();
            slot.UpdataSlotInfo(data);
        }
    }

    /// <summary>
    /// 最终透明度，动画持续时间
    /// </summary>
    /// <param name="endAlpha"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    IEnumerator FadeRoutine(float endAlpha, float duration, bool isShow)   //效果上移并修改透明度。
    {
        float startAlpha = canvasGroup.alpha;
        Vector2 offest = new Vector2(0f, 100f);
        Vector2 startPos = isShow ? originalPosition - offest : originalPosition;
        Vector2 endPos = isShow ? originalPosition : originalPosition - offest;

        float elapsed = 0f;
        while (elapsed < duration) 
        {
            elapsed += Time.unscaledDeltaTime;
            float percent = elapsed / duration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, percent);

            startPos = Vector2.Lerp(startPos, endPos, percent);
            rt.anchoredPosition = startPos;

            yield return null;
        }
        rt.anchoredPosition = endPos;
        canvasGroup.alpha = endAlpha;

        if(!isShow) gameObject.SetActive(false);
    }
}
