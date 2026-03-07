using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArtFighterSkill_1 : Skill
{
    public float startDamageTime;//触发伤害时间
    private float timeCounter;
    GameObject parent;
    LeaderController controller;
    private float lifeTimeCounter;//生命周期

    private SkillTrigger mySkillTrigger;

    private void Awake()
    {
        parent = transform.parent.gameObject;
        controller = transform.parent.gameObject.GetComponent<LeaderController>();
        mySkillTrigger = GetComponentInChildren<SkillTrigger>();
    }

    private void OnEnable()
    {
        controller.anim.SetBool("Is_Skill_2", true);
        lifeTimeCounter = skillStats[spellAbility].duration;
    }

    private void Update()
    {
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
        }
        if (timeCounter > 0)
        {
            //每次攻击范围内敌人不为空时播放技能动画——之后只要敌人不为空，则循环
            //每次攻击范围内敌人不为空时并且首次timeCounter归零时，触发伤害——之后只要敌人不为空，则循环
            timeCounter -= Time.deltaTime;
            if (timeCounter <= 0)
            {
                Debug.Log("技能1命中敌人");
                timeCounter = skillStats[spellAbility].timeBetweenAttacks; //设置攻击间隔
                int attackAmount = Mathf.Min(skillStats[spellAbility].amount, mySkillTrigger.enemies.Count); //可攻击敌人数量   取两者中的最小值
                for (int i = 0; i < attackAmount; i++)
                {
                    mySkillTrigger.enemies[i].TakeDamage(skillStats[spellAbility].damage);
                    Debug.Log("对敌人：" + mySkillTrigger.enemies[i].name + " 造成了：" + skillStats[spellAbility].damage + "点伤害");
                }
                Debug.Log("攻击到的敌人数量为：" + attackAmount);
            }
        }

        lifeTimeCounter -= Time.deltaTime;
        if (lifeTimeCounter <= 0) 
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        Debug.Log("out skill_2");
        controller.haveSkillIsActivation = false;//技能结束后将技能组释放状态置为false
        controller.normalAttack.SetActive(true);
        controller.anim.SetBool("Is_Skill_2", false);
        controller.currentTrigger = controller.skillTriggers[LeaderController.skillType.attack];
    }
}

//技能指示器