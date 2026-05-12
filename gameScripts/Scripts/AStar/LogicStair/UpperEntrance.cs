using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperEntrance : MonoBehaviour
{
    private StairArea stair;

    // Start is called before the first frame update
    void Start()
    {
        stair = GetComponentInParent<StairArea>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Controllable" || collision.tag == "Enemy") 
        {
            if (stair != null) 
            {
                stair.HandleChildTrigger(collision, gameObject);
            }
        }
    }
}
