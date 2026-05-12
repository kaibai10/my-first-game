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
    private bool is_Show = false;   //地图是否正在显示

    public LineRenderer roudeLine;

    public Image sideImageBG;
    public Image sideImage;
    public AnimationCurve showCurve;
    public AnimationCurve hideCurve;

    public Transform wayPointHander;
    public GameObject wayPointPrefab;

    public Canvas canvas;

    [Header("Line Settings")]
    public float lineWidth = 5f;
    [Tooltip("决定路线最大的弯曲偏离像素距离(UI通常需要100~300)")]
    public float maxWaveHeight = 150f;
    [Tooltip("弯曲的杂乱程度/频率(建议1~5)")]
    public float curveComplexity = 3f;

    private Vector2 PanelUPPosition;
    private Vector2 PanelBOTTOMPosition;
    private GameObject parentsCanvas;
    private bool inMainTown = true;
    void Start()
    {
        GameObject startPointGameObject = GameObject.Find("Start Point");
        GameObject endPointGameObject = GameObject.Find("End Point");
        startPoint = startPointGameObject.GetComponent<RectTransform>().anchoredPosition;
        endPoint = endPointGameObject.GetComponent<RectTransform>().anchoredPosition;
        GenerateRandomPointsWithFixedSumX_Proportional(startPoint,endPoint, roudeCount, 170f);  //获取移动路线

        RectTransform rt = GetComponent<RectTransform>();
        PanelUPPosition = rt.anchoredPosition + new Vector2(0, rt.rect.height);
        PanelBOTTOMPosition = rt.anchoredPosition;

        parentsCanvas = transform.parent.gameObject;
        DontDestroyOnLoad(parentsCanvas);
        InitLine();
    }

    // Update is called once per frame
    void Update()
    {
        if (canvas.worldCamera == null)
        {
            Debug.Log("原摄像机为null");
            canvas.worldCamera = Camera.main;
            if (canvas.worldCamera == null)
            {
                Debug.LogWarning("新场景没有 Tag 为 MainCamera 的摄像机！");
            }
        }

        //如果当前段没有绘制完成，则禁止一切用户交互
        if (currentSegment == false) return;

        if (Input.GetKeyDown(KeyCode.M)) ShowMap(false);
        //if (Input.GetKeyDown(KeyCode.N)) Move();
    }

    //跳转到相应目标场景
    public void LoadScene() 
    {
        if (currentRoudeIndex == 7 || currentRoudeIndex == 14)
            SceneManager.LoadScene("SampleScene");
        else
            SceneManager.LoadScene("SingleTeamCombatScene");
        HidePanel();
    }

    //地图跳转到事件触发场景   单选面板会覆盖在地图面板之上，需要调整。
    public void ShowMap(bool moving) 
    {
        if (!is_Show) ShowPanel();
        else HidePanel();

        //如果条件合适（前往下一个路径点），则等待1.5秒后待MapPanel完全显示完成后再开启路径点动画
        if (moving) Invoke("Move", 1.5f); 
    }

    //移动到下个路径点
    public void Move()
    {
        if (currentSegment == true && currentRoudeIndex < roudeCount)
        {
            StopAllCoroutines();
            StartCoroutine(MoveSequence());
        }
    }

    private void HidePanel()
    {
        StopAllCoroutines();
        StartCoroutine(HidePanel_(PanelBOTTOMPosition));
        is_Show = false;
    }

    private void ShowPanel() 
    {
        StopAllCoroutines();
        StartCoroutine(ShowPanel_(PanelUPPosition));
        is_Show = true;
    }

    //LineRender的初始化
    private void InitLine()
    {
        //设置端点数量和初始点位置
        roudeLine.positionCount = 1;
        roudeLine.SetPosition(0, currentRoudePoints[0]);

        roudeLine.material = new Material(Shader.Find("UI/Default")); // UI 专用材质
        roudeLine.useWorldSpace = false;

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

    /// <summary>
    /// 生成自然弯曲的地图路径点 (Perlin噪声 + 包络线)
    /// </summary>
    public void GenerateRandomPointsWithFixedSumX_Proportional(Vector3 startP, Vector3 targetP, int pointCount, float waveHeight)
    {
        Vector3[] points = new Vector3[pointCount + 1];

        // 强行锁定起点和终点
        points[0] = startP;
        points[pointCount] = targetP;

        // 获取起终点方向向量，以此计算出【法线】（即垂直于该直线的方向）
        Vector3 direction = (targetP - startP).normalized;
        Vector3 normal = new Vector3(-direction.y, direction.x, 0f); // 2D垂直向量

        // 使用随机种子来采样噪声
        float seedX = Random.Range(0f, 1000f);
        float seedY = Random.Range(0f, 1000f);

        for (int i = 1; i < pointCount; i++)
        {
            float t = i / (float)pointCount; // 0~1 的进度

            // 基础线上的点 (直直的过去)
            Vector3 basePoint = Vector3.Lerp(startP, targetP, t);

            // 【核心1：包络线 Envelope】
            // 使用 Mathf.Sin(t * PI) 确保 t=0(起点) 和 t=1(终点) 时，偏移量逼近为0，线条在两头会被拉平无缝连接
            float envelope = Mathf.Sin(t * Mathf.PI);

            // 【核心2：柏林噪声 Perlin Noise】
            // 叠加一层基础起伏和一层细节起伏，取值范围映射到 -1 到 1 之间
            float noise1 = Mathf.PerlinNoise(seedX + t * curveComplexity, seedY) * 2f - 1f;
            float noise2 = Mathf.PerlinNoise(seedY + t * (curveComplexity * 2f), seedX) * 2f - 1f;

            float combinedNoise = (noise1 * 0.7f) + (noise2 * 0.3f); // 二者混合

            // 最终坐标 = 直辖上的点 + 垂直反方向偏移 * 混合弯曲度 * 限制两头不要断联的包络比例 * 用户设定的最大振幅
            points[i] = basePoint + normal * (combinedNoise * envelope * waveHeight);
        }

        currentRoudePoints = points;
    }

    IEnumerator ShowPanel_(Vector2 targetPos)
    {
        RectTransform rt = GetComponent<RectTransform>();
        Vector2 startPos = rt.anchoredPosition;

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

    IEnumerator HidePanel_(Vector2 targetPos) 
    {
        RectTransform rt = GetComponent<RectTransform>();
        Vector2 startPos = rt.anchoredPosition;

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
        //第一次调用时进入主城，不需要前进
        if (inMainTown)
        {
            currentSegment = false;
            yield return  StartCoroutine(ShowCurrentPos());

            inMainTown = false;
            currentSegment = true;
            yield break;
        }
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
        GameObject newWayPoint = Instantiate(wayPointPrefab, wayPointHander);
        newWayPoint.transform.localPosition = currentRoudePoints[currentRoudeIndex - 1];
        Image newWayPointSprite = newWayPoint.GetComponent<Image>();

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
        sideImageBG.gameObject.transform.localPosition = currentRoudePoints[currentRoudeIndex] + new Vector3(0f, 62.0f, 0f);
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

    /// <summary>
    /// 地图线段绘制
    /// </summary>
    /// <returns></returns>
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