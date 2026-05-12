using Spine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static LeaderController;

//挂载在角色上
public class SkillController : MonoBehaviour
{
    public List<SkillBase> equippedSkills;
    private Dictionary<int,float> cooldowns = new Dictionary<int,float>();
    private LeaderController leader;

    public bool haveSkillSelected = false;
    public int currentSelectedSkill = -1;  //当前选中的技能，-1默认当前没有选中技能
    public GameObject previewInstance;

    //同帧锁(记录最后一次释放技能的帧数)
    public int lastCastFrame = -1;

    private void Awake()
    {
        leader = gameObject.GetComponent<LeaderController>();
        cooldowns.Add(0, 0);
        cooldowns.Add(1, 0);
        cooldowns.Add(2, 0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) CastSkillByKey(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) CastSkillByKey(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) CastSkillByKey(2);

        if (haveSkillSelected && previewInstance != null)
        {
            HandleSkillPreview();
        }
    }

    //清除技能预览状态
    public void CleanupPreviewState() 
    {   
        if (previewInstance != null) Destroy(previewInstance);
        haveSkillSelected = false;
        previewInstance = null;
        currentSelectedSkill = -1;  //当前角色目前没有选中的技能
    }

    private void HandleSkillPreview()
    {
        // 预览跟随鼠标
        switch (equippedSkills[currentSelectedSkill].skillData.skillType) 
        {
            case SkillType.POINT_TARGET:    //跟随鼠标位置旋转
                Vector3 direction = Vector3.zero;
                direction = GetMousePos.instance.GetMousePosition() - leader.transform.position;
                float angle = Vector3.SignedAngle(Vector3.right, direction, Vector3.forward);
                previewInstance.transform.rotation = Quaternion.Euler(0f, 0f, angle);
                break;

            case SkillType.MOVABLE_AREA:    //跟随鼠标位置移动
                previewInstance.transform.position = GetMousePos.instance.GetMousePosition();
                break;
        }
        

        if (Input.GetMouseButtonDown(0)) // 左键释放
        {
            CastActiveSkill();
        }
        else if (Input.GetMouseButtonDown(1)) // 右键取消
        {
            CleanupPreviewState();
        }
    }

    //通过键盘按键释放技能
    public void CastSkillByKey(int skillSlot) 
    {
        if (haveSkillSelected) { Debug.Log("有其他技能已经被选中"); return; }
        if (IsOnCoolDown(skillSlot)) { Debug.Log("技能" + (skillSlot + 1) + "正在冷却"); return; }

        SkillData data = equippedSkills[skillSlot].skillData;

        // 关键点：根据配置决定父物体
        Transform parent = data.isFollowLeader ? transform : null;
        switch(data.skillType)
        {
            case SkillType.POINT_TARGET:
                previewInstance = Instantiate(data.skillPrefab, equippedSkills[skillSlot].transform.position, Quaternion.identity, parent);
                previewInstance.SetActive(true); break;

            case SkillType.MOVABLE_AREA:
                previewInstance = Instantiate(data.skillPrefab, GetMousePos.instance.GetMousePosition(), Quaternion.identity, parent);
                previewInstance.SetActive(true); break;
        }
        

        // 属性注入：确保实例知道谁是它的老大
        SkillBase sb = previewInstance.GetComponent<SkillBase>();
        sb.leader = leader;
        sb.skillController = this;
        sb.skillSlot = skillSlot;
        sb.enabled = false; // 预览阶段停用逻辑

        haveSkillSelected = true;
        currentSelectedSkill = skillSlot;
    }

    private void CastActiveSkill()
    {
        if (previewInstance == null) return;

        // 启用技能逻辑，开始倒计时伤害
        previewInstance.GetComponent<SkillBase>().enabled = true;

        // 进入CD (在释放瞬间进入)
        if (!equippedSkills[currentSelectedSkill].skillData.skillFinishInCD)
            StartCoolDown(currentSelectedSkill, equippedSkills[currentSelectedSkill].skillData.stats[equippedSkills[currentSelectedSkill].skillData.skillLevel].cd);

        //标记释放了技能的这一帧
        lastCastFrame = Time.frameCount;

        // 断开预览关联，准备下一次准备
        previewInstance = null;
        haveSkillSelected = false;
        currentSelectedSkill = -1;
    }

    public void ActiveSkillAfterMove(SkillContext context) 
    {
        
    }

    //开始cd
    public void StartCoolDown(int skillSlot,float cooldown) 
    {
        if (!cooldowns.ContainsKey(skillSlot)) return;
        cooldowns[skillSlot] = Time.time + cooldown;
    }

    //技能是否处于cd状态
    public bool IsOnCoolDown(int skillSlot) 
    {
        if (!cooldowns.ContainsKey(skillSlot)) return false;
        return Time.time < cooldowns[skillSlot];
    }
}
