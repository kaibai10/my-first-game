using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetMousePos : MonoBehaviour
{
    public static GetMousePos instance;
    private void Awake()
    {
        instance = this;
    }

    private Vector3 mousePos;
    private Vector3 orginPoint;

    public Vector3 GetMousePosition() 
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }

    public RaycastHit2D GetMouseHit() 
    {
        RaycastHit2D hit = Physics2D.Raycast(orginPoint, Vector2.one, Vector2.Distance(orginPoint, mousePos));
        return hit;
    }
}
