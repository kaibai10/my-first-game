using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowCharacterInformationButton : MonoBehaviour
{
    public TMP_Text Damager, wulifangyu;
    public LeaderController signedLeader;

    void UpdataCharacterInform(LeaderController leader) 
    {
        Damager.text = "基础伤害值为：" + Damager;
        wulifangyu.text = "基础物理防御为：" + wulifangyu;
        signedLeader = leader;
    }

    void ShowCharacterInform() 
    {
        UpdataCharacterInform(signedLeader);
    }
}
