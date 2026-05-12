using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSkillTest : SkillBase
{
    public float startDamageTime;//触发伤害时间
    private float timeCounter;

    private void Awake()
    {
        leader = GetComponentInParent<LeaderController>();
        Initialize(skillData);
    }

    private void OnEnable()
    {
        leader.anim.SetBool("Is_Skill_3Begin", true);
        timeCounter = startDamageTime;
    }

    public override void Update()
    {
        //技能生命周期
        durationTimer -= Time.deltaTime;
        if (durationTimer <= 0)
        {
            FinishSkill();
        }

        ExecuteEffect();
    }

    //技能释放逻辑
    public override void ExecuteEffect() 
    {
        bool hasEnemies = skillTrigger.enemies != null && skillTrigger.enemies.Count > 0;
        leader.anim.SetBool("Is_Attacking", hasEnemies);
        Debug.Log("HaveEnemy的值为"+hasEnemies);
        //重置起始伤害时间
        if (hasEnemies == false)
            timeCounter = startDamageTime;

        //bool hasEnemies = skillTrigger.enemies != null && skillTrigger.enemies.Count > 0;
        if (hasEnemies) //开始攻击
        {
            leader.anim.SetBool("Is_Attacking", true);

            if (timeCounter > 0)
            {
                //每次攻击范围内敌人不为空时播放技能动画——之后只要敌人不为空，则循环
                //每次攻击范围内敌人不为空时并且首次timeCounter归零时，触发伤害——之后只要敌人不为空，则循环
                timeCounter -= Time.deltaTime;
                if (timeCounter <= 0)
                {
                    Debug.Log("技能3命中敌人");
                    timeCounter = skillData.stats[skillData.skillLevel].attackInterval; //设置攻击间隔
                    int attackAmount = Mathf.Min(skillData.stats[skillData.skillLevel].attackAmount, skillTrigger.enemies.Count); //可攻击敌人数量   取两者中的最小值
                    for (int i = 0; i < attackAmount; i++)
                    {
                        skillTrigger.enemies[i].TakeDamage(skillData.stats[skillData.skillLevel].damage);
                        Debug.Log("对敌人：" + skillTrigger.enemies[i].name + " 造成了：" + skillData.stats[skillData.skillLevel].damage + "点伤害");
                    }
                    Debug.Log("攻击到的敌人数量为：" + attackAmount);
                }
            }
        }
    }

    //技能结束逻辑
    public override void FinishSkill() 
    {
        //进入CD
        if (skillData.skillFinishInCD) skillController.StartCoolDown(skillSlot, skillData.stats[skillData.skillLevel].cd);

        skillController.CleanupPreviewState();

        if (isDestory == true) Destroy(gameObject);
        else gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        Debug.Log("out skill_3");
        leader.haveSkillIsActivation = false;
        leader.normalAttack.SetActive(true);
        leader.anim.SetBool("Is_Skill_3Begin", false);
        leader.currentTrigger = leader.skillTriggers[LeaderController.skillSlot.attack];
    }
}
