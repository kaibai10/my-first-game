using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCdController : MonoBehaviour
{
    public static SkillCdController instance;
    private void Awake()
    {
        instance = this;
    }

    //开启技能后需同时设置is_cd 和 timeCounter_cd;
    public void StartCd(Skill selectedSkill, float cd)
    {
        Debug.Log("开始cd");
        selectedSkill.is_cd = true;
        selectedSkill.timeCounter_cd = cd;

        StartCoroutine(StayCd(selectedSkill));
    }
    IEnumerator StayCd(Skill selectedSkill)
    {
        while (selectedSkill.timeCounter_cd > 0)
        {
            Debug.Log("正在cd");
            selectedSkill.timeCounter_cd -= Time.deltaTime;
            yield return null;
        }

        selectedSkill.timeCounter_cd = 0;
        selectedSkill.is_cd = false;
        Debug.Log("冷却结束可再次使用");
    }
}
