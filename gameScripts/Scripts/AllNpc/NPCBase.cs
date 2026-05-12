using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCBase : MonoBehaviour 
{
    public NPCType type;
    public LayerMask layer;

    public GameObject dialogBox;    //交互窗口
    private CanvasGroup canvasGroup;
    private Vector3 _uiOffset = new Vector3(0f, 2.5f, 0f);
    private Coroutine _activeCoroutine;

    protected bool isPlayerInRange = false;
    protected virtual void Awake()
    {
        dialogBox.SetActive(false); //起始状态设置为false
        canvasGroup = dialogBox.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    public virtual void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E)) 
        {
            eventTrigger();
        }
    }

    public virtual void eventTrigger() { } //子对象重写事件触发函数，触发对应事件


    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.tag == "Controllable") 
        {
            //显示交互框
            ToggleDialog(true);
            isPlayerInRange = true;
        }
    }
    public virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null && collision.tag == "Controllable")
        {
            //隐藏交互框
            ToggleDialog(false);
            isPlayerInRange = false;
        }
    }

    //对话框显隐切换
    private void ToggleDialog(bool show) 
    {
        if (_activeCoroutine != null) StopCoroutine(_activeCoroutine);
        _activeCoroutine = StartCoroutine(FadeRoutine(show ? 1f : 0f));
    }

    //对话框显隐切换动画
    IEnumerator FadeRoutine(float targetAlpha)
    {
        if (targetAlpha > 0) dialogBox.SetActive(true);

        float duration = 0.3f;  // 动画时长
        float elapsed = 0f;     // 计时器
        float startAlpha = canvasGroup.alpha;

        // 动画曲线（可选）：让效果更灵动
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float percent = elapsed / duration;

            // 更新透明度
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, percent);

            // 实时更新位置：保证对话框跟随 NPC
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + _uiOffset);
            // 增加一个随 alpha 变化的垂直偏移感
            float yOffset = Mathf.Lerp(-20f, 0f, canvasGroup.alpha);
            dialogBox.transform.position = (Vector2)screenPos + new Vector2(0, yOffset);

            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        if (targetAlpha <= 0) dialogBox.SetActive(false);
    }
}

public enum NPCType 
{
    Merchant,   //商人
    Doctor,     //医生
    Blacksmith  //铁匠
}