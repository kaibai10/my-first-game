using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;

public class NpcController : MonoBehaviour
{
    public static NpcController instance;
    private void Awake()
    {
        instance = this;
    }

    public LayerMask whatIsPlayer;
    public GameObject dialogueCanvas;
    public float dialogueRange;
    private bool is_dialog;

    private bool outOfRange;

    //移动相关属性
    public SkeletonMecanim skele;
    public Animator anim;
    public float moveSpeed;
    public float moveTime;
    private float moveTimerCounter;
    private float move_dire_x, move_dire_y;

    //定义Bounds包围盒
    Bounds movementBounds = new Bounds();
    Vector3 desiredPosition;

    //是否为可对话对象
    public bool is_Conversational;
    //是否为可移动对象
    public bool is_Movable;

    // Start is called before the first frame update
    void Start()
    {
        movementBounds.center = transform.position;
        movementBounds.extents = new Vector3(6f, 5f, 0f);

        outOfRange = true;
        is_dialog = false;

        moveTimerCounter = moveTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (is_Movable == true)
        {
            moveTimerCounter -= Time.deltaTime;
            if (moveTimerCounter <= 0.0f && is_dialog == false)
            {
                moveTimerCounter = moveTime;
                NpcMove();
            }


            desiredPosition = transform.position;
            desiredPosition += new Vector3(move_dire_x, move_dire_y, 0f) * moveSpeed * Time.deltaTime;

            //限制物体移动 (或者当移动到包围盒边缘时)
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, movementBounds.min.x, movementBounds.max.x);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, movementBounds.min.y, movementBounds.max.y);
            desiredPosition.z = Mathf.Clamp(desiredPosition.z, movementBounds.min.z, movementBounds.max.z);

            if (is_Conversational == true)
                DialogWindowController.instance.dialogueBack.transform.position += (desiredPosition - transform.position) * 45;
            transform.position = desiredPosition;
        }

        if (is_Conversational == true)
        {
            Collider2D Player = Physics2D.OverlapArea(transform.position + new Vector3(1, 1, 0) * dialogueRange, transform.position + new Vector3(-1, -1, 0) * dialogueRange, whatIsPlayer);
            if (Player != null && outOfRange == true)
            {
                DialogWindowController.instance.ShowDialogue();
                outOfRange = false;
            }
            else if (Player == null && outOfRange == false)
            {
                DialogWindowController.instance.HideDialogue();
                outOfRange = true;
            }
            if (Player != null)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    is_dialog = true;
                    transform.localScale = new Vector3(-1, 1, 1);
                    //对话结束后需要再调整缩放
                }
            }
        }
    }

    void NpcMove() 
    {
        float point = Random.Range(0.0f, 1.0f);
        if (point >= 0.0f && point <= 0.45f)
        {
            bool move_X = Random.Range(0.0f, 1.0f) > 0.5f ? true : false;
            if (move_X == true)
            {
                move_dire_x = Random.Range(0.0f, 1.0f) > 0.5f ? 1 : -1;
                skele.skeleton.ScaleX = move_dire_x == -1 ? -1f : 1f;
            }
            else
            {
                move_dire_y = Random.Range(0.0f, 1.0f) > 0.5f ? 1 : -1;
            }

            anim.SetBool("Is_Moving", true);
        }
        else
        {
            move_dire_x = 0f;
            move_dire_y = 0f;
            anim.SetBool("Is_Moving", false);
        }
    }
}
