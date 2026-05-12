using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

public class RoudeLine : MonoBehaviour
{
    public List<LineRenderer> roudeLines = new List<LineRenderer>();
    public List<LeaderController> leaders = new List<LeaderController>(); //从CharacterSelectSystem.instance.characterPrefabs获取

    [Header("摄像机设置")]
    public CinemachineVirtualCamera vcam;
    public float maxOrthoSize = 18.0f;
    public float introOrthoSize = 7.5f;
    public float normalOrthoSize = 12.0f;

    // Start is called before the first frame update
    void Start()
    {
        //入场时先禁用角色的LeaderController,等入场动画显示完成后再启用
        foreach (var lea in CharacterSelectSystem.instance.characterPrefabs)
        {
            LeaderController controller = lea.GetComponent<LeaderController>();
            leaders.Add(controller);
            controller.enabled = false;
        }
        foreach (var lea in leaders)
            lea.enabled = false;

        StartCoroutine(PlayIntroSequence());
    }

    //表现效果：摄像机初始高度为normalOrtho，开始时摄像机高度升高为maxOrthoSize，并且角色跟随line向上移动（最好是角色已经到位后，摄像机还没到达目标高度，在继续上移）
    //                                       等摄像机与角色到达第一阶段目标位置后，暂停0.5s角色先开始移动，随后摄像机Follow设置为队长，并将摄像机高度逐渐设置为introOrthoSize。
    //                                       等角色移动到目标位置后，摄像机高度再恢复为normalOrthoSize.
    IEnumerator PlayIntroSequence() 
    {
        //设置角色初始位置
        for (int i = 0; i < leaders.Count; i++) 
            leaders[i].transform.position = roudeLines[i].GetPosition(0);

        //摄像机初始化
        vcam.m_Lens.OrthographicSize = normalOrthoSize;
        vcam.Follow = null;
        vcam.transform.position = new Vector3(0f, 0f, -100f);

        // --- 第一阶段：角色上移 + 摄像机升高至 Max ---
        for (int i = 0; i < leaders.Count; i++)
        {
            StartCoroutine(MoveToTargetPoint(leaders[i], roudeLines[i].GetPosition(1)));
        }

        //按时间抬升摄像机高度(落后于角色移动到达目标点)
        float elapsed = 0f;
        float upDuration = 2.5f;
        while (elapsed < upDuration) 
        {
            elapsed += Time.deltaTime;
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(normalOrthoSize, maxOrthoSize, elapsed / upDuration);

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        // --- 第二阶段：设置摄像机跟随对象 + 角色继续移动 + 摄像机拉近至 Intro ---

        //角色先启动，记录完成移动的角色的数量
        int finishedCount = 0;
        for (int i = 0; i < leaders.Count; i++)
        {
            StartCoroutine(FollowIndividualPath(leaders[i], roudeLines[i], () => { finishedCount++; }));
        }

        //暂停等待角色启动后，再启动摄像机
        yield return new WaitForSeconds(0.25f);
        if (leaders.Count > 0) vcam.Follow = leaders[0].transform; // 镜头锁定队长

        //摄像机高度与角色移动一起进行，并在角色移动到目标点前到达自身目标值（先于角色到达目标点）
        yield return VcamMove(vcam.m_Lens.OrthographicSize, introOrthoSize, 1.2f);

        //当所有角色移动完成后，进入第三阶段
        while (finishedCount < leaders.Count)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.25f);

        // --- 第三阶段：恢复摄像机高度到 normalOrthoSize ---
        yield return VcamMove(vcam.m_Lens.OrthographicSize, normalOrthoSize, 1.2f);

        foreach (var leader in leaders)
            leader.enabled = true;

        Debug.Log("开场动画播放结束");
    }


    //摄像机高度调整线程
    IEnumerator VcamMove(float startOrthoSize, float targetOrthoSize,float duration) 
    {
        float elapsed = 0f;
        while (elapsed < duration) 
        {
            elapsed += Time.deltaTime;
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(startOrthoSize, targetOrthoSize, elapsed / duration);

            yield return null;
        }
    }

    //角色移动：只移动向目标点
    IEnumerator MoveToTargetPoint(LeaderController leader, Vector3 targetPos) 
    {
        leader.anim.SetBool("Is_Moving", true);

        while (Vector3.Distance(leader.transform.position, targetPos) > 0.05f) 
        {
            Vector3 dir = targetPos - leader.transform.position;
            leader.skele.skeleton.ScaleX = dir.x < 0 ? -1f : 1f;

            leader.transform.position = Vector3.MoveTowards(leader.transform.position, targetPos, 2.0f * Time.deltaTime);
            yield return null;
        }

        leader.anim.SetBool("Is_Moving", false);
    }

    //角色移动: 走完所有路径点
    IEnumerator FollowIndividualPath(LeaderController leader, LineRenderer line, System.Action onComplete) 
    {
        leader.anim.SetBool("Is_Moving", true);

        for (int i = 2; i < line.positionCount; i++) 
        {
            Vector3 TargetPos = line.GetPosition(i);

            //向目标点移动
            while (Vector3.Distance(leader.transform.position, TargetPos) > 0.05f) 
            {
                //计算移动方向并设置spine动画
                Vector3 dir = TargetPos - leader.transform.position;
                leader.skele.skeleton.ScaleX = dir.x < 0 ? -1f : 1f;

                leader.transform.position = Vector3.MoveTowards(leader.transform.position, TargetPos, 2.0f * Time.deltaTime);
                yield return null;
            }
        }

        leader.anim.SetBool("Is_Moving", false);    //移动结束后停止播放移动动画
        //移动完成后调用
        onComplete?.Invoke();
    }
}

//按角色走过的路径抬升摄像机高度，使角色和摄像机同步到达第一阶段
//float total_S = Vector3.Distance(roudeLines[0].GetPosition(0), roudeLines[0].GetPosition(1));
//while (leaders[0].transform.position != roudeLines[0].GetPosition(1))  
//{
//    vcam.m_Lens.OrthographicSize = Mathf.Lerp(normalOrthoSize, maxOrthoSize, 
//    total_S - (Vector3.Distance(leaders[0].transform.position, roudeLines[0].GetPosition(1)) / total_S));
//    yield return null;
//}