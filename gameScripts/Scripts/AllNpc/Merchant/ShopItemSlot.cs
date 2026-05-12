using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class ShopItemSlot : MonoBehaviour ,IPointerEnterHandler,IPointerExitHandler
{
    public ItemsData itemData;
    public TMP_Text itemName;
    public Image iconImage; //物品图标
    public TMP_Text priceText; //物品单价
    public Button slotButton;

    private ShopPanel parentsPanel;

    public void UpdateSlotInfo(ItemsData data, ShopPanel panel) 
    {
        itemData = data;
        itemName.text = data.itemName;
        iconImage.sprite = data.icon;
        priceText.text = data.price.ToString();
        slotButton = gameObject.GetComponent<Button>();

        parentsPanel = panel;

        slotButton.onClick.RemoveAllListeners();    //清除按钮上原有的监听事件
        //slotButton.onClick.AddListener(BuyItem);
        slotButton.onClick.AddListener(() => { GameEvents.TriggerPurchaseRequest(itemData); Debug.Log("发送购买物品请求：" + itemName.text); });
    }

    public void BuyItem() 
    {
        //触发购买请求
        GameEvents.TriggerPurchaseRequest(itemData);
        Debug.Log("发送购买物品请求：" + itemName.text);
    }

    public void OnPointerEnter(PointerEventData eventData) 
    {
        parentsPanel.ShowDescription(itemData, transform.position); //未设置显示动画
    }

    public void OnPointerExit(PointerEventData eventData) 
    {
        parentsPanel.HideDescription();
    }
}
