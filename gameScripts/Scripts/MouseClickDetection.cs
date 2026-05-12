using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class MouseClickDetection : MonoBehaviour
{
    public static MouseClickDetection instance;

    private void Awake()
    {
        instance = this;
    }

    [HideInInspector]
    public LeaderController currentLeader;
    public SkillController skillController;
    private LeaderController lastLeader;
    public LayerMask mask;
    private int clickTimes = 0;

    // Update is called once per frame
    void Update()
    {
        HandleLeftClickSelection();
        HandleMoveInput();
        HandleRightClickCancel();
    }

    //选中角色
    void HandleLeftClickSelection()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (skillController != null && (skillController.haveSkillSelected || skillController.lastCastFrame == Time.frameCount)) return;

        Vector3 mouseWorldPos = GetMousePos.instance.GetMousePosition();
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
        Vector2 orginPoint = mousePos2D - new Vector2(0.01f, 0.01f);
        RaycastHit2D hit = Physics2D.Raycast(orginPoint, Vector2.one, Vector2.Distance(orginPoint, mousePos2D), mask);

        if (hit.collider != null)
        {
            currentLeader = hit.collider.GetComponent<LeaderController>();
            skillController = hit.collider.GetComponent<SkillController>();
            clickTimes = 1;
        }
        else
        {
            lastLeader = currentLeader;
            clickTimes++;
        }
    }

    //获取移动目标位置
    void HandleMoveInput()
    {
        // 技能释放或键盘移动中时，不触发鼠标移动
        if (MouseClickEventsController.instance.selectedCharacter_Skill_Release || MouseClickEventsController.instance.character_Move_Key) return;
        // 【新增拦截】：这一帧刚释放了技能，禁止移动（非常关键）！
        if (skillController != null && skillController.lastCastFrame == Time.frameCount) return;
        // 有技能处于选中状态，不触发鼠标移动
        if (skillController.haveSkillSelected) return;  

        if (currentLeader != null && Input.GetMouseButtonDown(0))
        {
            if (clickTimes <= 1)
                return;

            // 防止点到 UI
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("点击了UI");
                return;
            }

            currentLeader.targetPosition = GetMousePos.instance.GetMousePosition();
        }
    }

    //取消选择
    void HandleRightClickCancel()
    {
        if (!Input.GetMouseButtonDown(1)) return;

        if (MouseClickEventsController.instance.selectedCharacter_Skill_Release)
            return;

        currentLeader = null;
        skillController = null;
        lastLeader = null;
        clickTimes = 0;
    }
}