using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject teamInterface;    //信息面板
    public UnityEngine.UI.Image leaderSideImage;//领队角色头像
    public UnityEngine.UI.Button[] controllableSideImage;//成员角色切换按钮
    public Controllable[] controllable;    //成员角色
    public UnityEngine.UI.Image characterillustration;//选中角色立绘
    private int selectedindex;//当前选中角色下标


    public GameObject membersView;  //成员视图
    public CanvasGroup canvasGroup; //控制透明度
    private bool is_ShowMembers;    //成员视图是否展开
    public float floatSpeed;    //浮动速度
    public float floatSpeedTime;//每次浮动时间
    private float speedTimeCounter;//计时器
    public AnimationCurve showCurve;
    public AnimationCurve hideCurve;//曲线
    private bool is_ShowCoroutines; //协程控制
    public UnityEngine.UI.Image unfoldImage;
     
    private void Start()
    {
        speedTimeCounter = 0;
        is_ShowMembers = false;
        UpdataCharacter_Side();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) 
        {
            if (teamInterface.activeSelf == false) 
                teamInterface.SetActive(true);
            else
                teamInterface.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            for (int i = 0; i < controllable.Length; i++)
            {   for (int j = i; j < controllable.Length; j++)
                    SimpleCharacterState.instance.CreateButton(controllable[j]);
            }
        }


        if (is_ShowMembers == true)
        {
            if (speedTimeCounter <= floatSpeedTime)
            {
                speedTimeCounter += Time.deltaTime;
                speedTimeCounter = Mathf.Clamp(speedTimeCounter, 0f - 0.01f, 1f + 0.01f);
                membersView.transform.position -= Vector3.up * floatSpeed * Time.deltaTime * 2;
                if (is_ShowCoroutines == false)
                {
                    is_ShowCoroutines = true;
                    StopAllCoroutines();
                    StartCoroutine(ShowMembers_());
                }
            }
        }
        else 
        {
            if (speedTimeCounter > 0) 
            {
                speedTimeCounter -= Time.deltaTime;
                speedTimeCounter = Mathf.Clamp(speedTimeCounter, 0f - 0.01f, 1f + 0.01f);
                membersView.transform.position += Vector3.up * floatSpeed * Time.deltaTime * 2;
                if (is_ShowCoroutines == true)
                {
                    is_ShowCoroutines = false;
                    StopAllCoroutines();
                    StartCoroutine(HideMembers_());
                }
            }
        }
    }

    void UpdataCharacter_Side()
    {
        for (int i = 0; i < controllableSideImage.Length; i++)
        {
            controllableSideImage[i].image.sprite = controllable[i].sideImage;
        }
        UpdataCharacter_illustration(controllable[0]);
        AttributeController.instance.UpdataCharacter_Attribute(controllable[0]);
    }

    void UpdataCharacter_illustration(Character theCharacter) 
    {
        characterillustration.sprite = theCharacter.illustration;
    }

    public void SelectCharacter() 
    {
        GameObject _click = EventSystem.current.currentSelectedGameObject;
        int index = 0;
        for (int i = 0; i < controllableSideImage.Length; i++) 
        {
            if (controllableSideImage[i].name == _click.name) 
            {
                break;
            }
            index++;
        }
        Debug.Log(_click.name);
        Debug.Log(controllable[index].name);
        AttributeController.instance.UpdataCharacter_Attribute(controllable[index]);
        UpdataCharacter_illustration(controllable[index]);
    }

    public void ShowAllMembers() 
    {
        if (is_ShowMembers == false)
        {
            is_ShowMembers = true;
        }
        else 
        {
            is_ShowMembers = false;
        }
    }

    IEnumerator ShowMembers_()
    {
        float timer = 0;
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha = showCurve.Evaluate(timer);
            unfoldImage.transform.rotation = Quaternion.Euler(new Vector3(showCurve.Evaluate(timer) * 180, 0, 0));
            timer += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator HideMembers_()
    {
        float timer = 0;
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha = hideCurve.Evaluate(timer);
            unfoldImage.transform.rotation = Quaternion.Euler(new Vector3(hideCurve.Evaluate(timer) * 180, 0, 0));
            timer += Time.deltaTime;
            yield return null;
        }
    }
}