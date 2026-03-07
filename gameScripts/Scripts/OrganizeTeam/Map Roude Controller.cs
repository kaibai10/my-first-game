using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;

public class MapRoudeController : MonoBehaviour
{
    public static MapRoudeController instance;
    private void Awake()
    {
        instance = this;
    }

    private Vector2 startPoint;
    private Vector2 endPoint;

    public int roudeCount;//路径点数量
    public int currentRoudeIndex = 0;
    public Vector3[] currentRoudePoints;
    public float drawSpeed;//逐渐画出线的速度;
    private bool currentSegment = true;//当前段是否绘制完成

    public LineRenderer roudeLine;

    public Image sideImageBG;
    public Image sideImage;
    public AnimationCurve showCurve;
    public AnimationCurve hideCurve;

    public GameObject wayPointPrefab;

    public Canvas canvas;

    void Start()
    {
        GameObject startPointGameObject = GameObject.Find("Start Point");
        GameObject endPointGameObject = GameObject.Find("End Point");
        RectTransform rt = GetComponent<RectTransform>();
        startPoint = startPointGameObject.transform.position - new Vector3(0f,rt.rect.height,0f);
        endPoint = endPointGameObject.transform.position - new Vector3(0f, rt.rect.height, 0f);

        GenerateRandomPointsWithFixedSumX_Proportional(endPoint, roudeCount, 1.6f);

        InitLine();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Move();
        }
    }

    //地图跳转到事件触发场景
    public void Next() 
    {
        
    }

    public void HidePanel()
    {
        StopAllCoroutines();
        StartCoroutine(HidePanel_());
    }

    public void ShowPanel() 
    {
        StopAllCoroutines();
        StartCoroutine(ShowPanel_());
    }

    public void Move()
    {
        if (currentSegment == true && currentRoudeIndex < roudeCount)
        {
            StopAllCoroutines();
            StartCoroutine(MoveSequence());
        }
    }

    //LineRender的初始化
    private void InitLine()
    {
        //设置端点数量和初始点位置
        roudeLine.positionCount = 1;
        roudeLine.SetPosition(0, currentRoudePoints[0]);

        roudeLine.material = new Material(Shader.Find("UI/Default")); // UI 专用材质
        roudeLine.useWorldSpace = true;

        //平滑效果
        //roudeLine.numCornerVertices = 1;     // 转角圆润度（越大越圆）
        //roudeLine.numCapVertices = 2;        // 端点圆润度

        // 设置线段的起点颜色和终点颜色
        roudeLine.startColor = Color.blue;
        roudeLine.endColor = Color.red;

        // 设置线段起点宽度和终点宽度
        roudeLine.startWidth = 0.1f;
        roudeLine.endWidth = 0.1f;
    }

    public void GenerateRandomPointsWithFixedSumX_Proportional(Vector2 targetPoint, int pointCount, float maxWaveHeight = 1.8f)
    {
        Vector3[] points = new Vector3[pointCount + 1];
        float totalX = targetPoint.x - startPoint.x;

        points[0] = new Vector3(startPoint.x, startPoint.y, -5f);
        points[pointCount] = new Vector3(targetPoint.x, targetPoint.y, -5f);

        // 随机种子，保证每次不同但可控
        float seed = Random.Range(0f, 100f);

        for (int i = 1; i < pointCount; i++)
        {
            float t = i / (float)pointCount;  // 0~1 的进度

            // 主 X 坐标（线性前进）
            float x = startPoint.x + t * totalX;

            // ========== 关键：三层波浪叠加，模拟真实地图路径 ==========
            // 第1层：大波浪（整体趋势，1~2个大弯）
            float bigWave = Mathf.Sin(t * Mathf.PI * 1.3f + seed) * maxWaveHeight * 0.5f;

            // 第2层：中波浪（局部起伏，3~5个小弯）
            float midWave = Mathf.Sin(t * Mathf.PI * 5.5f + seed * 1.7f) * maxWaveHeight * 0.25f;

            // 第3层：极细微噪声（增加真实感，但不破坏平滑）
            //float tinyNoise = (Mathf.PerlinNoise(seed + t * 18f, seed * 0.5f) - 0.5f) * maxWaveHeight * 0.15f;

            // 最终 Y 偏移 = 大波浪主导 + 中波浪点缀 + 微噪修饰
            float yOffset = bigWave + midWave;

            // 保证不偏离起点终点太多（插值平滑）
            float baseY = Mathf.Lerp(startPoint.y, targetPoint.y, t);
            float y = baseY + yOffset;

            points[i] = new Vector3(x, y, -5f);
        }

        currentRoudePoints = points;
    }

    IEnumerator ShowPanel_()
    {
        RectTransform rt = GetComponent<RectTransform>();
        Vector2 startPos = rt.anchoredPosition;
        Vector2 targetPos = startPos + new Vector2(0, rt.rect.height); // 向下移动一个自身高度

        float duration = 1.0f;     // 希望的动画总时长
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            startPos = Vector2.Lerp(startPos, targetPos, 0.02f);
            rt.anchoredPosition = startPos;
            yield return null;
        }

        rt.anchoredPosition = targetPos; // 确保最终位置精确
    }

    IEnumerator HidePanel_() 
    {
        RectTransform rt = GetComponent<RectTransform>();
        Vector2 startPos = rt.anchoredPosition;
        Vector2 targetPos = startPos - new Vector2(0, rt.rect.height); // 向下移动一个自身高度

        float duration = 1.0f;     // 希望的动画总时长
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            startPos = Vector2.Lerp(startPos, targetPos, 0.02f);
            rt.anchoredPosition = startPos;
            yield return null;
        }

        rt.anchoredPosition = targetPos; // 确保最终位置精确
    }

    IEnumerator MoveSequence()
    {
        //几个协程按顺序依次进行
        yield return StartCoroutine(HideCurrentPos());
        currentRoudeIndex++;
        yield return StartCoroutine(DrawLineGradually());


        Coroutine showPos = StartCoroutine(ShowCurrentPos());
        if (currentRoudeIndex - 1 != 0) 
        {
            Coroutine showPoint = StartCoroutine(ShowWayPoint());
            yield return showPoint;
        }
        

        yield return showPos;
    }

    IEnumerator ShowWayPoint()
    {
        GameObject newWayPoint = Instantiate(wayPointPrefab, currentRoudePoints[currentRoudeIndex-1], Quaternion.identity);
        SpriteRenderer newWayPointSprite = newWayPoint.GetComponent<SpriteRenderer>();

        newWayPoint.SetActive(true);
        Color color = newWayPointSprite.color;
        float timer = 0;
        while (color.a < 1)
        {
            timer += Time.deltaTime;
            color.a = showCurve.Evaluate(timer);

            newWayPointSprite.color = color;
            yield return null;
        }

        color.a = 1.0f;
        newWayPointSprite.color = color;
    }

    IEnumerator ShowCurrentPos() 
    {
        sideImageBG.gameObject.transform.position = currentRoudePoints[currentRoudeIndex] + new Vector3(0f, 0.65f, 0f);
        Color color = sideImageBG.color;
        float timer = 0;
        while (color.a < 1) 
        {
            timer += Time.deltaTime;
            color.a = showCurve.Evaluate(timer);

            sideImageBG.color = color;
            sideImage.color = color;
            yield return null;
        }

        color.a = 1.0f;
        sideImageBG.color = color;
        sideImage.color = color;
    }

    IEnumerator HideCurrentPos()
    {
        Color color = sideImageBG.color;
        float timer = 0;
        while (color.a > 0)
        {
            timer += Time.deltaTime;
            color.a = hideCurve.Evaluate(timer);

            sideImageBG.color = color;
            sideImage.color = color;
            yield return null;
        }
        color.a = 0.0f;
        sideImageBG.color = color;
        sideImage.color = color;
    }

    IEnumerator DrawLineGradually() 
    {
        Debug.Log("开始绘制第" + (currentRoudeIndex + 1) + "段线段");
        currentSegment = false;

        roudeLine.positionCount = Mathf.Max(roudeLine.positionCount, currentRoudeIndex + 1);

        //当前段的长度
        float currentSegmentDistance = Vector3.Distance(currentRoudePoints[currentRoudeIndex - 1], currentRoudePoints[currentRoudeIndex]);
        //已经画出的长度
        float distanceDrawn = 0;

        while (distanceDrawn < currentSegmentDistance) 
        {
            distanceDrawn += drawSpeed * Time.deltaTime;
            float t = Mathf.Clamp01(distanceDrawn / currentSegmentDistance);
            Vector3 currentPos = Vector3.Lerp(currentRoudePoints[currentRoudeIndex - 1], currentRoudePoints[currentRoudeIndex], t);

            roudeLine.SetPosition(currentRoudeIndex, currentPos);

            Debug.Log("正在画线：" + currentRoudeIndex + "坐标为:(" + currentPos.x + "," + currentPos.y + "," + currentPos.z + ")");
            yield return null;
        }

        //确保最后一帧的位置精确无误
        roudeLine.SetPosition(currentRoudeIndex, currentRoudePoints[currentRoudeIndex]);

        Debug.Log("第" + (currentRoudeIndex + 1) + "段线段绘制完成");
        currentSegment = true;
    }
}