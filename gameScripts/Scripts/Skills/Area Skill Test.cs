using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSkillTest : Skill
{
    public float startDamageTime;//触发伤害时间
    private float timeCounter;
    public GameObject parent;
    public LeaderController controller;
    private float lifeTimeCounter;//生命周期
    private bool resetTimeCounter = true;

    private SkillTrigger mySkillTrigger;

    private void Awake()
    {
        mySkillTrigger = GetComponentInChildren<SkillTrigger>();
    }

    private void OnEnable()
    {
        controller.anim.SetBool("Is_Skill_3Begin", true);
        lifeTimeCounter = skillStats[spellAbility].duration;
    }

    private void Update()
    {
        Attack();

        if (mySkillTrigger.enemies != null && mySkillTrigger.enemies.Count > 0)
        {
            if (controller.anim.GetBool("Enemies_Survival") == false)
            {
                controller.anim.SetBool("Enemies_Survival", true);
                timeCounter = startDamageTime;
            }
        }
        else if (mySkillTrigger.enemies == null || mySkillTrigger.enemies.Count <= 0)
        {
            controller.anim.SetBool("Enemies_Survival", false);
            timeCounter = 0;
        }

        if (timeCounter > 0)
        {
            //每次攻击范围内敌人不为空时播放技能动画——之后只要敌人不为空，则循环
            //每次攻击范围内敌人不为空时并且首次timeCounter归零时，触发伤害——之后只要敌人不为空，则循环
            timeCounter -= Time.deltaTime;
            if (timeCounter <= 0)
            {
                Debug.Log("技能3命中敌人");
                timeCounter = skillStats[spellAbility].timeBetweenAttacks; //设置攻击间隔
                int attackAmount = Mathf.Min(skillStats[spellAbility].amount, mySkillTrigger.enemies.Count); //可攻击敌人数量   取两者中的最小值
                for (int i = 0; i < attackAmount; i++)
                {
                    mySkillTrigger.enemies[i].TakeDamage(skillStats[spellAbility].damage);
                    Debug.Log("对敌人：" + mySkillTrigger.enemies[i].name+" 造成了：" + skillStats[spellAbility].damage + "点伤害");
                }
                Debug.Log("攻击到的敌人数量为：" + attackAmount);
            }
        }

        lifeTimeCounter -= Time.deltaTime;
        if (lifeTimeCounter <= 0)
        {
            if (isDestory == true)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);
        }
    }

    private void Attack() 
    {
        bool hasEnemies = mySkillTrigger.enemies != null && mySkillTrigger.enemies.Count > 0;
        controller.anim.SetBool("Enemies_Survival", hasEnemies);

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

    private void OnDisable()
    {
        //controller.SetAllBool(controller.anim, false);
        Debug.Log("out skill_3");
        controller.haveSkillIsActivation = false;
        controller.normalAttack.SetActive(true);
        controller.anim.SetBool("Is_Skill_3Begin", false);
        controller.currentTrigger = controller.skillTriggers[LeaderController.skillType.attack];
    }
}
