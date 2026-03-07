using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleTeamCombatUIController : MonoBehaviour
{
    public GameObject panel;
    private Vector3 characterPos;
    private Vector3 panelPos;
    public Vector3 movePos;

    public Button showMemberStatsButton;
    public GameObject infoPanel;

    //照搬UIController中的配置
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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (MouseClickDetection.instance.currentLeader != null)
        {
            ShowSelectCharactPanel();
            panel.SetActive(true);
        }
        else 
        {
            panel.SetActive(false);
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

    void ShowSelectCharactPanel() 
    {
        characterPos = MouseClickDetection.instance.currentLeader.transform.position;
        panelPos = characterPos + movePos;
        Vector3 panelScenePos = Camera.main.WorldToScreenPoint(panelPos);
        panel.transform.position = panelScenePos;
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
