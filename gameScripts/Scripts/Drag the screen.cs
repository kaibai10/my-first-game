using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dragthescreen : MonoBehaviour
{
    public float dragSpeed = 2f; //拖拽速度
    public float smoothTime = 0.1f; //平滑时间
    public bool enableInertia = true;//启用惯性
    public float inertiaDeceleration = 2f;//惯性减速
    private Vector3 velocity;//速度

    private bool isDragging = false;
    public float longPressDuration = 0.20f;//长按检测
    private float timeCounter;

    private Vector3 dragOrigin;//拖拽起始坐标
    private Vector3 targetPosition;//拖拽终点坐标
    public Camera cam;
    private bool longPressTrigger = false;

    // Update is called once per frame
    void Update()
    {
        MouseDrag();

        if (isDragging == false && enableInertia == true && velocity.magnitude > 0.05f) 
        {
            ApplyInertia();
        }
    }

    private void MouseDrag()
    {
        if (Input.GetMouseButtonDown(1)) 
        {
            timeCounter = 0;
            isDragging = false;
            velocity = Vector3.zero;
            Debug.Log("右键按下");
        }

        if (Input.GetMouseButton(1)) 
        {
            Debug.Log("右键长按");
            timeCounter += Time.deltaTime;
            if ((timeCounter >= longPressDuration && longPressTrigger == false)) 
            {
                longPressTrigger = true;
                isDragging = true;
                dragOrigin = GetMousePos.instance.GetMousePosition();
                Debug.Log("拖拽触发");
            }

            if (isDragging == true) 
            {
                ContinueDrag();
            }
        }

        if (Input.GetMouseButtonUp(1)) 
        {
            if (isDragging == true)
            {
                isDragging = false;
            }
            time = 0;
            timeCounter = 0;
            longPressTrigger = false;

            Debug.Log("右键抬起");
        }
    }

    void ContinueDrag()
    {
        if (isDragging == true) 
        {
            Vector3 currentMousepos = GetMousePos.instance.GetMousePosition();
            Vector3 move = dragOrigin - currentMousepos;

            targetPosition = cam.transform.position + move * dragSpeed * Time.deltaTime;
            cam.transform.position = targetPosition;

            if (enableInertia == true) 
            {
                velocity = move / Time.deltaTime;
            }

            dragOrigin = currentMousepos;
        }
    }

    private float time;

    void ApplyInertia() 
    {
        time += Time.deltaTime;
        targetPosition += velocity * Time.deltaTime;
        //cam.transform.position = targetPosition;
        cam.transform.position = Vector3.SmoothDamp(cam.transform.position, targetPosition,ref velocity, smoothTime);
        velocity = Vector3.Lerp(velocity, Vector3.zero, inertiaDeceleration * time);
        Debug.Log("惯性移动位置：" + cam.transform.position + ", 速度: " + velocity.magnitude);  
    }

    //Vector3 GetMouseWorldPosition()
    //{
    //    Vector3 mousePos = Input.mousePosition;
    //    mousePos.z = -42f; // 尝试固定的深度值
    //    Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
    //    Debug.Log($"屏幕坐标: {Input.mousePosition} -> 世界坐标: {worldPos}");
    //    return worldPos;
    //}
}


//if (Input.GetMouseButton(1))
//{

//    timeCounter += Time.deltaTime;
//    if (timeCounter >= longPressDuration)
//    {
//        Debug.Log("激活"+ timeCounter);
//        isDragging = true;
//        dragOrigin = GetMouseWorldPosition();
//        Debug.Log("dragOrigin: " + dragOrigin);
//        velocity = Vector3.zero;
//    }

//    if (isDragging == true)
//    {
//        Vector3 currentPos = GetMouseWorldPosition();
//        Debug.Log("currentPos: " + currentPos);
//        Vector3 move = dragOrigin - currentPos; //currentPos - dragOrigin; 为拖拽向量，返过来为camera移动方向
//        Debug.Log("move: " + move);

//        //targetPosition = transform.position + move;
//        //transform.position = targetPosition;
//        targetPosition = cam.transform.position + move;
//        cam.transform.position = targetPosition;
//        Debug.Log("拖动位置：" + cam.transform.position);
//        if (enableInertia)
//        {
//            velocity = move / Time.deltaTime;
//        }
//    }
//}

////处理惯性移动
//if (isDragging == false)
//{
//    if (enableInertia == true && velocity.magnitude > 0)
//    {
//        Debug.Log("进入惯性处理阶段");
//        targetPosition += velocity * Time.deltaTime;
//        velocity = Vector3.Lerp(velocity, Vector3.zero, inertiaDeceleration * Time.deltaTime);

//        cam.transform.position = Vector3.SmoothDamp(cam.transform.position, targetPosition, ref velocity, smoothTime);
//        Debug.Log("平滑移动位置：" + cam.transform.position);
//    }
//}

//if (Input.GetMouseButtonUp(1))
//{
//    isDragging = false;
//    timeCounter = 0;
//}