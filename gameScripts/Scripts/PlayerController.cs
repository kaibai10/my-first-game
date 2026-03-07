using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    private void Awake()
    {
        instance = this;
    }

    public Animator anim;
    public float moveSpeed;
    public SkeletonMecanim skele;

    //控制interact动画播放结束之后转换为relax状态
    float duration = 2.66f;
    float timer;

    public Rigidbody2D rig;
    Vector3 moveInput = new Vector3(0f, 0f, 0f);
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");


        if (moveInput != Vector3.zero)
        {
            anim.SetBool("Is_sleeping", false);
            anim.SetBool("Is_siting", false);
            anim.SetBool("Is_interact", false);
            anim.SetBool("Is_moving", true);
            skele.skeleton.ScaleX = moveInput.x < 0 ? -1f : 1f;
        }
        else 
        {
            anim.SetBool("Is_moving", false);
        }

        if (Input.GetKeyDown(KeyCode.Z)) 
        {
            if (anim.GetBool("Is_sleeping") == false)
            {
                anim.SetBool("Is_siting", false);
                anim.SetBool("Is_interact", false);
                anim.SetBool("Is_moving", false);
                anim.SetBool("Is_sleeping", true);
            }
            else 
            {
                anim.SetBool("Is_sleeping", false);
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            if (anim.GetBool("Is_siting") == false)
            {
                anim.SetBool("Is_sleeping", false);
                anim.SetBool("Is_interact", false);
                anim.SetBool("Is_moving", false);
                anim.SetBool("Is_siting", true);
            }
            else
            {
                anim.SetBool("Is_siting", false);
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (anim.GetBool("Is_interact") == false)
            {
                anim.SetBool("Is_sleeping", false);
                anim.SetBool("Is_siting", false);
                anim.SetBool("Is_moving", false);
                anim.SetBool("Is_interact", true);
                timer = duration;
            }
            else
            {
                anim.SetBool("Is_interact", false);
            }
        }
        if (anim.GetBool("Is_interact") == true) 
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                anim.SetBool("Is_interact", false);
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 position = rig.transform.position;
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");
        moveInput = moveInput.normalized;
        position += moveInput * moveSpeed * Time.deltaTime;

        rig.MovePosition(position);
    }
}
