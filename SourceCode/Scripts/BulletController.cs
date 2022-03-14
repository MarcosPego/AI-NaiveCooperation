using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= 4.3)
        {
            transform.position = new Vector3(0, 38.6f, 7);
        }
        else
        {
            transform.position += new Vector3(0, -0.02f, 0);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        transform.position = new Vector3(0, 38.6f, 7);
    }

}
