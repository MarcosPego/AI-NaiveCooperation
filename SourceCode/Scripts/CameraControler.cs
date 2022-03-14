using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControler : MonoBehaviour
{
    public Vector3 offset;
    public Vector3 target;
    public float playersDistance;

    // Start is called before the first frame update
    void Start()
    {
        /*offset = target - transform.position;
        Debug.Log(target);
        Debug.Log(transform.position);*/
    }

    // Update is called once per frame
    void Update()
    {

        transform.position = target - offset;
        if (playersDistance >= 5)  transform.position += playersDistance * Vector3.right;
        else transform.position += 5 * Vector3.right;
        //transform.LookAt(target);
    }
}
