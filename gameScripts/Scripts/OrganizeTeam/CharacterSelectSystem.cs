using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectSystem : MonoBehaviour
{
    public static CharacterSelectSystem instance;
    private void Awake()
    {
        instance = this;
    }

    public List<string> careers = new List<string> { "guards", "defenders", "caster", "sniper", "supporter", "medic" };
    public List<LeaderController> guards;       //近卫
    public List<LeaderController> defenders;    //盾卫
    public List<LeaderController> caster;       //术士
    public List<LeaderController> sniper;       //狙击
    public List<LeaderController> supporter;    //辅助
    public List<LeaderController> medic;        //医疗

    public string lastCareer;   //更新前的职业
    public string currentCareer;//更新后的职业
    public TMP_Text currentCareerName;

    public List<LeaderController> currentCharacterList;//选中职业的角色列表
    public LeaderController currentCharacter;//当前选中的角色
    public TMP_Text currentCharacterNameText;

    public GameObject characterToggleParent;
    public Toggle characterPrefab;

    public List<CareerToggle> careerToggles;

    public List<CharacterToggle> characterToggles;
    public List<Toggle> charactertoggles;

    public Dictionary<string, string> careerCareer = new Dictionary<string, string>
    {
        ["guards"] = "近卫",
        ["defenders"] = "盾卫",
        ["caster"] = "术士",
        ["sniper"] = "狙击",
        ["supporter"] = "辅助",
        ["medic"] = "医疗"
    };
    public Dictionary<string, int> careerIndex = new Dictionary<string, int>() 
    {
        ["guards"] = 0,
        ["defenders"] = 1,
        ["caster"] = 2,
        ["sniper"] = 3,
        ["supporter"] = 4,
        ["medic"] = 5
    };
    public Dictionary<string, List<LeaderController>> careerCharacter = new Dictionary<string, List<LeaderController>>();

    public GameObject teamSelectPanel;
    public GameObject careerPanel;

    //存储完全体的角色预制体(当前可以控制的角色)
    public List<GameObject> characterPrefabs = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        careerCharacter["guards"] = guards;
        careerCharacter["defenders"] = defenders;
        careerCharacter["caster"] = caster;
        careerCharacter["sniper"] = sniper;
        careerCharacter["supporter"] = supporter;
        careerCharacter["medic"] = medic;

        currentCareer = "guards";
        currentCharacterList = careerCharacter[currentCareer];
        currentCharacter = currentCharacterList[0];

        for (int i = 0; i < currentCharacterList.Count; i++)
        {
            Toggle newCharacterBox = Instantiate(characterPrefab, characterToggleParent.transform);
            newCharacterBox.gameObject.SetActive(true);
            CharacterToggle newCharacterScript = newCharacterBox.GetComponent<CharacterToggle>();

            newCharacterBox.onValueChanged.AddListener(newCharacterScript.ActivedToggle);
            TMP_Text tmpInGameObject = newCharacterBox.GetComponentInChildren<TMP_Text>();

            newCharacterScript.characterNameText = tmpInGameObject;
            newCharacterScript.assignedCharacter = currentCharacterList[0];

            characterToggles.Add(newCharacterScript);
            charactertoggles.Add(newCharacterBox);

            newCharacterScript.UpdateCharacter(currentCharacterList[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < careerToggles.Count; i++) 
        {
            careerToggles[i].UpdateCareer(careers[i]);
        }

        //当队伍中所有角色均已设置完成后才能进入下一步(显示地图)
        SelectedCharacter.instance.ShowSelectCompleteButton();
    }

    public void UpdateCareerList()
    {
        currentCareerName.text = currentCareer; //更新

        //更新显示的角色列表 

        foreach (Toggle toggle in charactertoggles)
        {
            Debug.Log("销毁");
            Destroy(toggle.gameObject);
        }
        characterToggles.Clear();
        charactertoggles.Clear();
        UpdateCharacterList(); //currentCharacterList
    }

    //更新角色列表
    public void UpdateCharacterList() //List<LeaderController> theCharacterList
    {
        currentCharacterList = careerCharacter[currentCareer];
        currentCharacter = currentCharacterList[0];
        for (int i = 0; i < currentCharacterList.Count; i++) 
        {
            Toggle newCharacterBox = Instantiate(characterPrefab, characterToggleParent.transform);
            if (newCharacterBox.gameObject.activeSelf == false) 
            {
                newCharacterBox.gameObject.SetActive(true);
            }
            CharacterToggle newCharacterScript = newCharacterBox.GetComponent<CharacterToggle>();

            newCharacterBox.onValueChanged.AddListener(newCharacterScript.ActivedToggle);

            TMP_Text tmpInGameObject = newCharacterBox.GetComponentInChildren<TMP_Text>();
            newCharacterScript.characterNameText = tmpInGameObject;
            //newCharacterScript.assignedCharacter = currentCharacterList[i];

            characterToggles.Add(newCharacterScript);
            charactertoggles.Add(newCharacterBox);

            newCharacterScript.UpdateCharacter(currentCharacterList[i]);
        }
    }

    //点击确认
    public void clickConfirm() 
    {
        teamSelectPanel.gameObject.SetActive(true);
        SelectedCharacter.instance.AddInleaderControllers(currentCharacter);
        SelectedCharacter.instance.UpdatePanel();

        //若未将角色/技能选择界面归位则，则调用GoToNextPage()
        //if (PageTransition.instance.isNextPage == true) 
        //{
        //    PageTransition.instance.GoToNextPage();
        //}
        careerPanel.gameObject.SetActive(false);
    }

    //创建角色
    public void CreateCharacters() 
    {
        List<GameObject> prefabs = new List<GameObject>();
        var resourcesPath = Application.dataPath;
        var absolutePaths = System.IO.Directory.GetFiles(resourcesPath, "*.prefab", System.IO.SearchOption.AllDirectories);
        for (int i = 0; i < absolutePaths.Length; i++)
        {
            EditorUtility.DisplayProgressBar("获取预制体……", "获取预制体中……", (float)i / absolutePaths.Length);

            string path = "Assets" + absolutePaths[i].Remove(0, resourcesPath.Length);
            path = path.Replace("\\", "/");
            GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (prefab != null)
                prefabs.Add(prefab);
            else
                Debug.Log("预制体不存在！ " + path);
        }
        EditorUtility.ClearProgressBar();
        Debug.Log("预制体获取完成");

        foreach (LeaderController lea in SelectedCharacter.instance.leaderControllers) 
        {
            //prefabs.Contains(lea.characterName)
            foreach (GameObject mono in prefabs) 
            {
                if (mono.gameObject.name == lea.characterName)
                {
                    Debug.Log("在prefabs中成功查找到" + lea.gameObject.name + "预制体");
                    GameObject newGameObject = Instantiate(mono.gameObject);
                    DontDestroyOnLoad(newGameObject); //确保每个角色在创建后切换场景时不会被销毁
                    LeaderController newLeaderController = newGameObject.GetComponent<LeaderController>();  //获取创建的角色对象上的LeaderController组件
                    foreach (Skill skill in lea.skills)
                    {
                        if (skill != null)
                        {
                            GameObject newSkillObject = Instantiate(skill.gameObject, newGameObject.transform); //将技能物体作为子物体添加到角色物体上
                            Skill newSkill = newSkillObject.GetComponent<Skill>();  //获取技能对象上的Skill组件
                            newLeaderController.skills.Add(newSkill);
                            Debug.Log("成功为角色" + newGameObject.name + "添加技能" + skill.name);
                        }
                    }
                    Debug.Log("角色" + newGameObject.name + "技能添加完毕");
                    characterPrefabs.Add(newGameObject);
                }
            }
        }
        Debug.Log("所有角色技能均已添加完毕");
    }
}