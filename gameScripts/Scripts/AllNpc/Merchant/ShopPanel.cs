using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public RectTransform rt; // 存放商店内容的容器，用于做位移动画
    private Vector2 originalPosition; // 记录面板的初始位置(显示时的位置)
    public Button closeButton;

    [Header("商品生成配置")]
    public GameObject itemSlotPrefab;   // 商品按钮预制体
    public RectTransform contentFolder; // 存放商店内容的容器，用于做缩放动画,对应你代码中的 contentFolder

    [Header("简介面板配置")]
    public GameObject descriptionPanel; // 简介面板物体
    public TextMeshProUGUI descNameText;
    public TextMeshProUGUI descInfoText;

    private void Awake()
    {
        descriptionPanel.SetActive(false);
        closeButton.onClick.AddListener(Hide);
    }

    //测试用
    private void OnEnable()
    {
        GameEvents.OnPurchaseRequest += BuyItem;
    }
    private void OnDisable()
    {
        GameEvents.OnPurchaseRequest -= BuyItem;
    }


    public void BuyItem(ItemsData data) 
    {
        Debug.Log("成功购买到商品：" + data.itemName);
    }
    public void Show(List<ItemsData> itemsDatas) 
    {
        gameObject.SetActive(true);
        UpdateInventory(itemsDatas);

        StopAllCoroutines();
        StartCoroutine(FadeRoutine(1f, 0.2f, true));
    }

    public void Hide() 
    {
        gameObject.SetActive(false);
        //StopAllCoroutines();
        //StartCoroutine(FadeRoutine(0f, 0.2f, false));
    }

    private void UpdateInventory(List<ItemsData> itemsDatas) 
    {
        Debug.Log("更新商店列表");
        // 1、清除商店原物品
        foreach (Transform child in contentFolder) 
        {
            Destroy(child.gameObject);
        }

        // 2、生成新商品
        foreach (var data in itemsDatas) 
        {
            GameObject obj = Instantiate(itemSlotPrefab, contentFolder.transform);
            obj.SetActive(true);
            ShopItemSlot slot = obj.GetComponent<ShopItemSlot>();
            slot.UpdateSlotInfo(data, this);
        }
    }

    IEnumerator FadeRoutine(float targetAlpha, float duration, bool isShow) 
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

            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, percent);
            startPos = Vector2.Lerp(startPos, endPos, percent);
            rt.anchoredPosition = startPos;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        rt.anchoredPosition = endPos;
        if (!isShow) { gameObject.SetActive(false); }
    }

    // --- 简介面板控制 ---           未设置显示动画
    public void ShowDescription(ItemsData data, Vector3 position)
    {
        descriptionPanel.SetActive(true);
        descNameText.text = data.itemName;
        descInfoText.text = data.productDescription;

        // 让简介面板出现在商品按钮旁边
        descriptionPanel.transform.position = position + new Vector3(300, -100, 0);
    }

    public void HideDescription()
    {
        descriptionPanel.SetActive(false);
    }
}
