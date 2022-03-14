using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NaivePlayer : MonoBehaviour
{
    // Start is called before the first frame update

    //public CharacterController controller;
    public Vector3 spawn;
    public Rigidbody rb;
    
    public float moveSpeed;

    public float jumpForce;


    public int jumpsLeft;
    public int maxJumpCount;

    //private Collider[] isGrounded;
    private Vector3 moveDirection;

    public LayerMask whatIsBoundaryDeath;

    public float checkRadius;
    public LayerMask whatIsGround;
    public bool isActive = true;

    public float isGroundedRayLength = 0.05f;

    public GameObject manager;

    public bool isGrounded
    {
        get
        {
            Vector3 position = transform.position;
            position.y = GetComponent<Collider>().bounds.min.y + 0.1f;
            float length = isGroundedRayLength + 0.1f;
            Debug.DrawRay(position, Vector3.down * length, Color.red, 20, true);
            bool grounded = Physics.Raycast(position, Vector3.down, length, whatIsGround.value);
            return grounded;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        spawn = GetComponent<Transform>().position;
    }

    // Update is called once per frame
    void Update()
    {
        //isGrounded = Physics.OverlapSphere(feetPos.position, checkRadius, whatIsGround, QueryTriggerInteraction.Collide);

        if (isActive)
        {
            float moveInput = Input.GetAxis("Horizontal");
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, moveInput * moveSpeed);
            if (isGrounded)
            {
                jumpsLeft = maxJumpCount;
            }

            if (jumpsLeft > 0 && Input.GetButtonDown("Jump"))
            {
                rb.velocity = rb.velocity + Vector3.up * jumpForce;
                jumpsLeft--;
            }
        }

        //velocity try
        /*isGrounded = Physics.OverlapSphere(feetPos.position, checkRadius, whatIsGround, QueryTriggerInteraction.Collide);

        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector3(moveInput * moveSpeed, rb.velocity.y, rb.velocity.z);

        if(isGrounded.Length > 0 && Input.GetButtonDown("Jump"))
        {
            rb.velocity = rb.velocity +  Vector3.up * jumpForce; 
        }*/


        //controller try
        /*if (controller.isGrounded)
        {
            

            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpFroce ;
            }
        }


        moveDirection.y = moveDirection.y + (Physics.gravity.y * gravityScale * Time.deltaTime);
        controller.Move(moveDirection * Time.deltaTime);*/

    }

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "Death")
        {
            Die();
        }

    }

    void OnTriggerEnter(Collider other)
    {

        int whatIsBoundaryDeathNumber = (int)Mathf.Log(whatIsBoundaryDeath.value, 2);
        if (other.gameObject.layer == whatIsBoundaryDeathNumber)
        {
            Die();
        }

        if (other.transform.tag == "Victory")
        {
            Victory();
        }
    }

    void Die()
    {
        //Application.LoadLevel(Application.loadedLevel);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Victory()
    {
        manager.GetComponent<TimeDisplay>().pause = true;
        Debug.Log("GG eazy! Git good m9");
    }
}
