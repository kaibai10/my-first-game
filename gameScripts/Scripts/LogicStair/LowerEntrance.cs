using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerEntrance : MonoBehaviour
{
    private StairArea parent;

    // Start is called before the first frame update
    void Start()
    {
        parent = GetComponentInParent<StairArea>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Controllable" || collision.tag == "Enemy")
        {
            if (parent != null)
            {
                parent.HandleChildTrigger(collision, gameObject);
            }
        }
    }
}
