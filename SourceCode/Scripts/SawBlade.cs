using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBlade : MonoBehaviour
{
    // Start is called before the first frame update
    public float rotationRate = 5f;
    public Animator anim;
    public float waitTime = 5f;
    
    


    /*IEnumerator Go()
    {
        while (true)
        {
            anim.SetTrigger("ChangeDir");
            yield return new WaitForSeconds(waitTime);
        }

    }*/

    void Start()
    {
        //StartCoroutine(Go());
    }




    // Update is called once per frame
    void Update()
    {
        
        transform.GetChild(0).RotateAround(transform.GetChild(0).position, Vector3.right, rotationRate * Time.deltaTime);
    }


}
