using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoctorNPC : NPCBase
{
    protected override void Awake()
    {
        base.Awake();

        // 动态查找场景中所有 LeaderController 实例，自动填充 hateTable
        LeaderController[] leaders = FindObjectsOfType<LeaderController>();
        foreach (var gameObject in leaders) //CharacterSelectSystem.instance.characterPrefabs
        {
            LeaderController leader = gameObject;
            AllCharacter.leaderControllers.Add(leader);
        }
    }

    public override void Update() 
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E)) eventTrigger();
    }

    public override void eventTrigger()
    {
        //显示页面
        Debug.Log("医生：发送打开医疗面板请求");
        GameEvents.TriggerOpenMedicalPanelRequest();
    }
}
