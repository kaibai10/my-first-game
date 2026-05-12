using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MedicalSlot : MonoBehaviour
{
    public LeaderController leader;
    public Image sideImage;//头像框 ，状态栏 ， 治疗费用
    private Button slotButton;

    private void Awake()
    {
        slotButton = GetComponent<Button>();
    }

    public void UpdataSlotInfo(LeaderController leader)
    {
        this.leader = leader;
        sideImage.sprite = leader.sideImage;

        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(() => { Debug.Log("触发治疗请求"); GameEvents.TriggerTreatRequest(leader); });
    }

    //治疗
    public void Treat() 
    {
        GameEvents.TriggerTreatRequest(leader);
    }
}
