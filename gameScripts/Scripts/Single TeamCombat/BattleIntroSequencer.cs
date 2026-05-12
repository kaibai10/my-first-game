using Cinemachine;
using System.Collections;
using UnityEngine;

public class BattleIntroSequencer : MonoBehaviour
{
    public CinemachineVirtualCamera vcamIntro;
    public CinemachineVirtualCamera vcamBattle;
    public Rigidbody2D rb;

    private void Start()
    {
        Debug.Log("特写镜头");  
        Invoke("ShowIntro", 1.0f);
    }

    public void ShowIntro() 
    {
        StartCoroutine(IntroRoutine());
    }

    public IEnumerator IntroRoutine() 
    {
        //1、禁用玩家操作

        //开始战斗时为特写镜头
        vcamIntro.Priority = 20;
        vcamBattle.Priority = 10;

        yield return new WaitForSeconds(0.8f);

        vcamIntro.Priority = 10;
        vcamBattle.Priority = 20;

        float elapsed = 0;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.up * 2.0f;

        while (elapsed < 1.0f) 
        {
            elapsed += Time.deltaTime;

            // 简单的插值移动
            rb.velocity = Vector2.up * 3.0f;

            yield return null;
        }

        rb.velocity = Vector2.zero;
        //2、恢复玩家操作
    }
}
