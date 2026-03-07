using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPath : MonoBehaviour
{
    [Header("视觉设置")]
    [SerializeField] private Color pathColor = Color.cyan;
    [SerializeField] private float lineWidth = 0.15f;
    [SerializeField] private float heightOffset = 0.05f;     // 让线稍微浮在地面上方，避免穿模

    [Header("动画效果（可选）")]
    [SerializeField] private bool animatePath = false;
    [SerializeField] private float animationSpeed = 2f;      // 路径出现动画速度

    private LineRenderer lineRenderer;
    private List<Vector3> currentPath = new List<Vector3>();
    private float animationProgress = 0f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupLineRenderer();
    }

    private void SetupLineRenderer()
    {
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // 简单材质
        lineRenderer.startColor = pathColor;
        lineRenderer.endColor = pathColor;
        lineRenderer.numCornerVertices = 5;     // 转角更圆滑（可选）
        lineRenderer.numCapVertices = 5;
    }

    /// <summary>
    /// 显示新的路径
    /// </summary>
    /// <param name="path">A* 返回的路径节点列表（从起点到终点）</param>
    public void ShowPathLine(List<MyNode> path)
    {
        if (path == null || path.Count == 0)
        {
            ClearPath();
            return;
        }

        // 转换为世界坐标，并加上一点高度偏移
        currentPath.Clear();
        foreach (var node in path)
        {
            Vector3 pos = node.pos;
            pos.y += heightOffset;
            currentPath.Add(pos);
        }

        if (!animatePath)
        {
            // 直接显示完整路径
            UpdateLineImmediate();
        }
        else
        {
            // 开始动画
            animationProgress = 0f;
        }
    }

    /// <summary>
    /// 立即更新整条路径（无动画）
    /// </summary>
    private void UpdateLineImmediate()
    {
        lineRenderer.positionCount = currentPath.Count;
        lineRenderer.SetPositions(currentPath.ToArray());
    }

    private void Update()
    {
        if (animatePath && currentPath.Count > 0 && animationProgress < 1f)
        {
            animationProgress += Time.deltaTime * animationSpeed;
            animationProgress = Mathf.Clamp01(animationProgress);

            int visiblePoints = Mathf.CeilToInt(currentPath.Count * animationProgress);
            visiblePoints = Mathf.Clamp(visiblePoints, 0, currentPath.Count);

            lineRenderer.positionCount = visiblePoints;
            for (int i = 0; i < visiblePoints; i++)
            {
                lineRenderer.SetPosition(i, currentPath[i]);
            }
        }
    }

    /// <summary>
    /// 清除当前显示的路径
    /// </summary>
    public void ClearPath()
    {
        lineRenderer.positionCount = 0;
        currentPath.Clear();
        animationProgress = 0f;
    }

    //// 可选：路径更新时自动跟随角色移动（如果角色已经在移动中） 
    //public void LateUpdate()
    //{
    //    if (currentPath.Count > 0 && lineRenderer.positionCount > 0)
    //    {
    //        // 可以让路径第一个点跟随角色当前位置
    //        Vector3 startPos = transform.position + Vector3.up * heightOffset;
    //        lineRenderer.SetPosition(0, startPos);
    //    }
    //}

    // 调试用：在场景中画出 Gizmos
    private void OnDrawGizmos()
    {
        if (currentPath.Count < 2) return;

        Gizmos.color = pathColor;
        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            Gizmos.DrawLine(currentPath[i], currentPath[i + 1]);
        }
    }
}
