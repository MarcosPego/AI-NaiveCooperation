using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlock : MonoBehaviour
{
    bool ascending = false;
    public bool col = false;
    public float topPos;
    public float botPos;


    void Start()
    {
        botPos = transform.position.y;
        topPos = transform.position.y + 5;
    }

    void FixedUpdate()
    {
        if (!col)
        {
            if (transform.position.y <= botPos)
            {
                ascending = true;

            }
            if (transform.position.y >= topPos)
            {
                ascending = false;
            }

            if (ascending)
            {
                transform.position += new Vector3(0, 0.05f, 0);
            }
            else
            {
                transform.position += new Vector3(0, -0.25f, 0);
            }
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        col = true;
    }


}
