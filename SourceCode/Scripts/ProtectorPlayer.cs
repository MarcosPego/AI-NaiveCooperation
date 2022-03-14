using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProtectorPlayer : MonoBehaviour
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

    public float checkRadius;
    public LayerMask whatIsGround;

    public LayerMask whatIsBoundaryDeath;

    public bool isActive = false;


    public float isGroundedRayLength = 0.05f;

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
        if(isActive)
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

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Vector3 scaleChange = new Vector3(1, 1, 1);
                this.gameObject.transform.localScale = scaleChange;
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Vector3 scaleChange = new Vector3(1, 3, 1);
                this.gameObject.transform.localScale = scaleChange;
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Vector3 scaleChange = new Vector3(1, 1, 3);
                this.gameObject.transform.localScale = scaleChange;
            }
        }
;

    }

    void OnCollisionEnter(Collision other)
    {
        int whatIsBoundaryDeathNumber = (int)Mathf.Log(whatIsGround.value, 2);
        if (other.gameObject.layer == whatIsBoundaryDeathNumber)
        {
            this.GetComponent<ProtectorAgent>().isColliding = true;
        }
    }

    void OnCollisionExit(Collision other)
    {
        int whatIsBoundaryDeathNumber = (int)Mathf.Log(whatIsGround.value, 2);
        if (other.gameObject.layer == whatIsBoundaryDeathNumber)
        {
            this.GetComponent<ProtectorAgent>().isColliding = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {

        int whatIsBoundaryDeathNumber = (int) Mathf.Log(whatIsBoundaryDeath.value, 2);
        if (other.gameObject.layer == whatIsBoundaryDeathNumber)
        {
            Die();
        }

        if (other.gameObject.tag == "Naive")
        {
            this.GetComponent<ProtectorAgent>().avoidNaive = true;
        }

    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Naive")
        {
            this.GetComponent<ProtectorAgent>().avoidNaive = false;
        }
    }

    void Die()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameObject.SetActive(false);
    }


}
