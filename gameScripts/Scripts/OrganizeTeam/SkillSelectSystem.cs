using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectSystem : MonoBehaviour
{
    public static SkillSelectSystem instance;
    private void Awake()
    {
        instance = this;
    }

    public Dictionary<string,CareerSkills> skillsController = new Dictionary<string,CareerSkills>();
    public CareerSkills guardsSkills;
    public CareerSkills defendersSkills;
    public CareerSkills casterSkills;
    public CareerSkills sniperSkills;
    public CareerSkills supporterSkills;
    public CareerSkills medicSkills;

    public string currentCareer;
    public int currentskillIndex;   //要选择的技能是技能几
    public List<Button> currentSkillsList;//当前显示的技能列表，更新时需清除后重新添加
    public List<Skill> targetShowSkillSet;//要显示的技能列表
    public Button skillPrefab;
    public GameObject skillPrefabParent;

    public List<CharacterSkillButton> selectedSkillButtonList; //角色的三个技能列表

    public GameObject characterSkillBox;
    public GameObject SkillSelectionBox;

    // Start is called before the first frame update
    void Start()
    {
        skillsController["guards"] = guardsSkills;
        skillsController["defenders"] = defendersSkills;
        skillsController["caster"] = casterSkills;
        skillsController["sniper"] = sniperSkills;
        skillsController["supporter"] = supporterSkills;
        skillsController["medic"] = medicSkills;

        currentCareer = CharacterSelectSystem.instance.currentCareer;
        currentskillIndex = 0;

        UpdateCurrentSkillList();
    }

    // Update is called once per frame
    void Update()
    {


    }

    public void UpdateCurrentSkillList()
    {
        //更新前先清除List列表
        foreach (Button button in currentSkillsList) 
        {
            Destroy(button.gameObject);
        }
        currentSkillsList.Clear();

        if (currentskillIndex == 0)
            targetShowSkillSet = skillsController[CharacterSelectSystem.instance.currentCareer].skillSet1;
        else if (currentskillIndex == 1)
            targetShowSkillSet = skillsController[CharacterSelectSystem.instance.currentCareer].skillSet2;
        else if (currentskillIndex == 2)
            targetShowSkillSet = skillsController[CharacterSelectSystem.instance.currentCareer].skillSet3;

        foreach (Skill skill in targetShowSkillSet)
        {
            Button newSelectionSkillButton = Instantiate(skillPrefab, skillPrefabParent.transform);
            if (newSelectionSkillButton.gameObject.activeSelf == false)
            {
                newSelectionSkillButton.gameObject.SetActive(true);
            }
            SkillSelectButton newSelectionSkillScript = newSelectionSkillButton.GetComponent<SkillSelectButton>();

            //将Button自带的Image属性赋值到，SkillSelectButton脚本中的Image属性上
            Image theButtonImage = newSelectionSkillButton.GetComponent<Image>();
            newSelectionSkillScript.skillIcon = theButtonImage;

            //为生成的新Button按钮添加监听事件
            newSelectionSkillButton.onClick.AddListener(newSelectionSkillScript.ActivedButton);

            //更新图像不再像CharacterSelectSystem那样放在update函数中，修改为创建时就调用
            newSelectionSkillScript.UpdateButtonInfo(skill);

            //将按钮放入到存储显示按钮的List列表中
            currentSkillsList.Add(newSelectionSkillButton);
        }
    }
}


[System.Serializable]
public class CareerSkills
{
    public List<Skill> skillSet1;
    public List<Skill> skillSet2;
    public List<Skill> skillSet3;
}