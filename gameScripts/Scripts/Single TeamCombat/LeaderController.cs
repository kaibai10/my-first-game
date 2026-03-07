using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LeaderController : Character
{
    public bool is_Leader;
    private Vector3 moveInput = new Vector3(0f, 0f, 0f);

    private Vector3 toward;
    private float angle;
    private float initialXoffset, initialYoffset;
    [Tooltip("触发器y值偏移量")]
    public float targetYoffsetDifference;
    [Tooltip("首次伤害触发时间")]
    public float startDamageTime;//首次伤害触发时间
    [Tooltip("攻击间隔")]
    public float timeBetweenAttacks;//攻击间隔
    private float timeCounter = 0;
    private bool resetTimeCounter = true;

    //鼠标控制参数
    [HideInInspector]
    public Vector3 targetPosition = new Vector3(0f, 0f, 0f);

    //当前激活的SkillTrigger
    [HideInInspector]
    public SkillTrigger currentTrigger;
    public enum skillType
    { attack, skill_1, skill_2, skill_3 }
    public Dictionary<skillType, SkillTrigger> skillTriggers = new Dictionary<skillType, SkillTrigger>();

    [HideInInspector]
    public skillType nextSkill;//移动到目标位置后要释放的技能
    [HideInInspector]
    public float nextAngle;//要释放的技能的角度

    [HideInInspector]
    public Vector3 nextPosition;//范围型技能释放的中心
    [HideInInspector]
    public GameObject nextPrefab;//范围型技能预制体

    //是否有技能处于触发状态（避免同时多个技能被触发）
    [HideInInspector]
    public bool haveSkillIsActivation = false;

    private AStar aStar;
    private ShowPath showPath;

    private void Start()
    {
        initialXoffset = skills[0].transform.localPosition.x;
        initialYoffset = skills[0].transform.localPosition.y;

        skillTriggers.Add(skillType.attack, normalAttack.GetComponentInChildren<SkillTrigger>());
        skillTriggers.Add(skillType.skill_1, skills[0].GetComponentInChildren<SkillTrigger>());
        skillTriggers.Add(skillType.skill_2, skills[1].GetComponentInChildren<SkillTrigger>());
        skillTriggers.Add(skillType.skill_3, skills[2].GetComponentInChildren<SkillTrigger>());

        //默认为普通攻击
        currentTrigger = skillTriggers[skillType.attack];
    }

    private void Update()
    {
        if (is_Leader)
            Move();

        if (targetPosition == Vector3.zero) // 移動完成後
        {
            if (nextSkill != skillType.attack && nextAngle != 0)
            {
                NextAngleSkillActivation(nextAngle);
                ResetNextSkillData();
            }
            else if (nextSkill != skillType.attack && nextPosition != Vector3.zero)
            {
                NextPosSkillActivation(nextPosition);
                ResetNextSkillData();
            }
        }
        else
            MoveWithMouse(targetPosition);

        if (!anim.GetBool("Is_Moving"))
            Attack();

        KeySkillRotation();
        MouseSkillRotation();

        if (timeCounter > 0)
        {
            timeCounter -= Time.deltaTime;
            if (timeCounter <= 0)
            {
                timeCounter = timeBetweenAttacks;
                int num = Mathf.Min(attackAmount, currentTrigger.enemies.Count);
                for (int i = 0; i < num; i++)
                {
                    currentTrigger.enemies[i].TakeDamage(attackDamage);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ToggleSkill(skillType.skill_1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ToggleSkill(skillType.skill_2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ToggleSkill(skillType.skill_3);
        }
    }

    void MoveWithMouse(Vector3 targetMovePosition)
    {
        //获取移动距离和移动方向
        float moveDitance = Vector3.Distance(transform.position, targetMovePosition);
        Vector3 moveDirection = targetMovePosition - transform.position;

        if (moveDitance > 0.1f)
        {
            MouseClickEventsController.instance.character_Move_Mouse = true;
            anim.SetBool("Enemies_Survival", false);
            skele.skeleton.ScaleX = moveDirection.x < 0 ? -1f : 1f;
            anim.SetBool("Is_Moving", true);
            transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;
        }
        else
        {
            transform.position = targetMovePosition;
            anim.SetBool("Is_Moving", false);
            MouseClickEventsController.instance.character_Move_Mouse = false;

            //移动结束之后重置targetPosition
            targetPosition = Vector3.zero;
        }
    }

    //移动结束后的范围型技能释放
    void NextPosSkillActivation(Vector3 pos) 
    {
        GameObject prefab = Instantiate(nextPrefab, nextPosition, Quaternion.identity);
        prefab.SetActive(true);
    }

    //移动结束后的指向性技能释放
    void NextAngleSkillActivation(float angle) 
    {
        ToggleSkill(nextSkill);
        currentTrigger.transform.parent.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    //重置数据，避免重复触发
    void ResetNextSkillData() 
    {
        nextAngle = 0;
        nextPosition = Vector3.zero;
        nextPrefab = null;
        nextSkill = skillType.attack;
    }

    //键盘位置控制旋转  注：（下面这部分需要修改，每次只判断一个部分）
    void KeySkillRotation()
    {
        if (MouseClickEventsController.instance.character_Move_Key == true)
        {
            angle = Vector3.SignedAngle(Vector3.right, toward, Vector3.forward);
            currentTrigger.transform.parent.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            currentTrigger.transform.parent.gameObject.transform.localPosition = new Vector3(GetXpos(angle), initialYoffset + GetYpos(angle), 0f);
        }
    }
    //鼠标位移控制旋转
    void MouseSkillRotation()
    {
        if (MouseClickEventsController.instance.character_Move_Mouse == true && Input.GetMouseButtonDown(0) && MouseClickDetection.instance.currentLeader == this)
        {
            Vector3 mousePos = GetMousePos.instance.GetMousePosition();
            Vector3 direction = mousePos - transform.position;
            angle = Vector3.SignedAngle(Vector3.right, direction, Vector3.forward);
            currentTrigger.transform.parent.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            currentTrigger.transform.parent.gameObject.transform.localPosition = new Vector3(GetXpos(angle), initialYoffset + GetYpos(angle), 0f);
        }
    }
    float GetXpos(float angle)
    {
        if (angle >= 0 && angle <= 180)
        {
            return Mathf.Lerp(-initialXoffset, initialXoffset, (180 - angle) / 180);
        }
        else
        {
            return Mathf.Lerp(-initialXoffset, initialXoffset, (180 + angle) / 180);
        }
    }
    float GetYpos(float angle)
    {
        if (angle <= 90 && angle >= -90)
        {
            return Mathf.Lerp(-targetYoffsetDifference, targetYoffsetDifference, (90 + angle) / 180);
        }
        else if (angle > 90 && angle <= 180)
        {
            return Mathf.Lerp(0, targetYoffsetDifference, (180 - angle) / 90);
        }
        else
        {
            return Mathf.Lerp(0, -targetYoffsetDifference, (angle - 180) / -90);
        }
    }


    //技能切换
    public void ToggleSkill(skillType type)
    {
        if (skillTriggers[type] != currentTrigger)
        {
            //关闭当前状态
            currentTrigger.transform.parent.gameObject.SetActive(false);

            //激活目标状态
            currentTrigger = skillTriggers[type];
            skillTriggers[type].transform.parent.gameObject.SetActive(true);
        }
    }

    void Attack()
    {
        bool hasEnemies = currentTrigger.enemies != null && currentTrigger.enemies.Count > 0;
        anim.SetBool("Enemies_Survival", hasEnemies);

        if (hasEnemies == false)
            resetTimeCounter = true;

        if (resetTimeCounter == true)
        {
            if (hasEnemies)
            {
                timeCounter = startDamageTime;
                resetTimeCounter = false;
            }
            else
            {
                timeCounter = 0;
            }
        }
    }

    void Move()
    {
        if (MouseClickEventsController.instance.selectedCharacter_Skill_Release != true && MouseClickEventsController.instance.character_Move_Mouse != true)
        {
            moveInput.x = Input.GetAxis("Horizontal");
            moveInput.y = Input.GetAxis("Vertical");
            moveInput = moveInput.normalized;

            anim.SetFloat("Move_X", moveInput.x);
            anim.SetFloat("Move_Y", moveInput.y);
            if (moveInput != Vector3.zero)
            {
                anim.SetBool("Enemies_Survival", false);
                skele.skeleton.ScaleX = moveInput.x < 0 ? -1f : 1f;
                anim.SetBool("Is_Moving", true);

                MouseClickEventsController.instance.character_Move_Key = true;
            }
            else
            {
                anim.SetBool("Is_Moving", false);
                MouseClickEventsController.instance.character_Move_Key = false;
            }

            if (moveInput != Vector3.zero)
            {
                toward = moveInput;
            }
            transform.position += moveInput * moveSpeed * Time.deltaTime;
        }
    }
}
