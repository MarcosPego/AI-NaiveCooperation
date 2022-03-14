using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NaiveAgent : MonoBehaviour
{
    [Range(0.0f, 100.0f)]
    public int alpha = 0;

    ///////////////////////////////////////////
    /// Movement Variables
    ///////////////////////////////////////////
    public Rigidbody rb;
    public float moveSpeed = 10;
    public float jumpForce = 2;
    public int jumpsLeft = 1;
    public int maxJumpCount = 1;
    public LayerMask whatIsGround;
    public float isGroundedRayLength = 0.05f;
    public bool isActive = false;

    ///////////////////////////////////////////
    /// Protector Variables
    ///////////////////////////////////////////
    public GameObject protectorAgent;
    public int DangerValue = 0;
    public bool shouldJump = false;
    public bool canJump = true;

    ///////////////////////////////////////////
    /// World Variables
    ///////////////////////////////////////////
    public List<Collider> victoryPoint = new List<Collider>();
    public List<Collider> wallsAndFloor = new List<Collider>();

    ///////////////////////////////////////////
    /// RayCast Variables
    ///////////////////////////////////////////
    public List<CardinalDirection> cardinalDirections = new List<CardinalDirection>();
    public float cardinalRangeVerHor = 1;
    public float cardinalRangeDiagonal = 5;
    public LayerMask maskIgnore;

    public struct CardinalDirection
    {
        public Vector3 vectorDir;
        public RaycastHit closestHitPoint;
        public bool canMove;
        public float cardinalRange;
    }

    public Vector3 victoryDir;

    ///////////////////////////////////////////
    /// Setup
    ///////////////////////////////////////////
    void Start()
    {

        if (PlayerPrefs.HasKey("alpha"))
        {
            alpha = PlayerPrefs.GetInt("alpha");
        }

        rb = GetComponent<Rigidbody>();

        //Getting all the world objects: walls and victory point
        var allObjectsWithTagWall = GameObject.FindGameObjectsWithTag("Wall");
        var allObjectsWithTagVictory = GameObject.FindGameObjectsWithTag("Victory");

        //Saving the colliders of the world objects
        foreach (GameObject wall in allObjectsWithTagWall)
        {
            wallsAndFloor.Add(wall.GetComponent<Collider>());
        }
        foreach (GameObject victory in allObjectsWithTagVictory)
        {
            victoryPoint.Add(victory.GetComponent<Collider>());
        }

        canJump = true;

        //Getting the location of the goal
        getVictoryDir();

        //Informing the protector agent of the direction of the goal
        informGoalDirection();

        //Defining the Raycasts
        addCardinalDirs();
        RayCastCardinalDirs();
    }

    ///////////////////////////////////////////
    /// Run
    ///////////////////////////////////////////
    void FixedUpdate()
    {
        if (isActive)
        {   
            //Check if agent is on the ground: resets the jump counter
            if (isGrounded)
            {
                jumpsLeft = maxJumpCount;
            }

            //Getting the location of the goal
            getVictoryDir();

            //Informing the protector agent of the direction of the goal
            informGoalDirection();

            //Defines the directions the agent can move to
            RayCastCardinalDirs();

            //Chooses the aproppriate action depending on what the agent perceives and wants to achieve
            Deliberate();
        }
    }

    ///////////////////////////////////////////
    /// Function responsible for the action decision. By taking the components of world and the protector agent, the naive agent 
    /// decides what's the best action to execute
    ///////////////////////////////////////////
    public void Deliberate() {
        //If the protector agent tells the naive agent that he should jump, then he jumps
        if (shouldJump) Jump();


        int chance = Random.Range(1, 101);

        //If there is no danger nearby, then the naive agent tries to move towards the goal
        if (DangerValue <= 0 || chance <= alpha) MoveWithRayCast();

        //Stops if the protector agent tells there are dangers
        else Idle();
    }

    ///////////////////////////////////////////
    /// Naive Agent Actions
    ///////////////////////////////////////////

    // -----------------------------
    // The naive agent just stays in the same place (still affected by world physics like gravity)
    // -----------------------------
    public void Idle()
    {
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
    }

    // -----------------------------
    // Naive agent just moves towards the goal - deprecated
    // -----------------------------
    public void MoveTowardsVictory()
    {
        Vector3 vectorDir = victoryPoint[0].bounds.center - transform.position;
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, Vector3.Normalize(vectorDir).z  * moveSpeed);
    }

    // -----------------------------
    // Given the vector that indicates where the goal is, the naive agent analyses the 
    // most direct possible path to take in that direction
    // -----------------------------
    public void MoveWithRayCast()
    {
        Vector3 bestDirection = new Vector3(0, 0, 0);
        float angleBetweenDirs = 180;

        foreach (CardinalDirection cd in cardinalDirections)
        {
            if (Vector3.Angle(cd.vectorDir, victoryDir) < angleBetweenDirs && cd.canMove)
            {
                bestDirection = cd.vectorDir;
                angleBetweenDirs = Vector3.Angle(cd.vectorDir, victoryDir);
            }
        }

        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, Vector3.Normalize(bestDirection).z * moveSpeed);
        if (bestDirection.y > 0 && jumpsLeft >0 && canJump)
        {
            rb.velocity = rb.velocity + Vector3.up * jumpForce;
            jumpsLeft--;
        }

    }

    // -----------------------------
    // The naive agent jumps
    // -----------------------------
    public void Jump()
    {
        //Checks if the agent can still jump, or if it's safe to jump (according to the protector agent)
        if (jumpsLeft > 0 && canJump)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpsLeft--;
        }
        shouldJump = false;
    }

    // -----------------------------
    //      Communication - Send
    // -----------------------------

    public void informGoalDirection()
    {
        protectorAgent.GetComponent<ProtectorAgent>().naiveDirection = victoryDir;
    }

    // -----------------------------
    //      Naive Agent Sensors
    // -----------------------------

    /*  
        Give the general direction between the agent and the victory point
     */
    public void getVictoryDir()
    {
        victoryDir = victoryPoint[0].bounds.center - transform.position;
    }

    /*  
    Allows to check if the Naive agent's can see the victory point directly
     */
    public bool RayCastVictory()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, victoryPoint[0].bounds.center - transform.position, out hit, 100000, ~maskIgnore))
        {
            if (hit.transform.tag == "Victory")
                return true;
            else
                return false;
        }
        return false;
    }

    /*  
        Allows to check the Naive agent's closest visible obstacles by raycasting in 8 different directions
         */
    public void RayCastCardinalDirs()
    {
        for (int i = 0; i < cardinalDirections.Count; i++)
        {
            CardinalDirection tempDir = new CardinalDirection();
            tempDir.vectorDir = cardinalDirections[i].vectorDir;
            tempDir.cardinalRange = cardinalDirections[i].cardinalRange;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, tempDir.vectorDir, out hit, tempDir.cardinalRange, ~maskIgnore))
            {
                tempDir.closestHitPoint = hit;
                tempDir.canMove = false;
            }
            else
            {
                tempDir.canMove = true;
                tempDir.closestHitPoint = hit;
            }
            cardinalDirections[i] = tempDir;
        }
    }

    // -----------------------------
    //      Adicional Functions
    // -----------------------------

    /*  
        Initializes all 8 percetion raycasts;
         */
    public void addCardinalDirs()
    {
        CardinalDirection tempDir = new CardinalDirection();
        tempDir.vectorDir = new Vector3(0, 1, 0);
        tempDir.canMove = true;
        tempDir.cardinalRange = cardinalRangeVerHor;
        cardinalDirections.Add(tempDir);

        tempDir = new CardinalDirection();
        tempDir.vectorDir = new Vector3(0, 0.25f, 0.75f);
        tempDir.canMove = true;
        tempDir.cardinalRange = cardinalRangeDiagonal;
        cardinalDirections.Add(tempDir);

        tempDir = new CardinalDirection();
        tempDir.vectorDir = new Vector3(0, 0, 1);
        tempDir.canMove = true;
        tempDir.cardinalRange = cardinalRangeVerHor;
        cardinalDirections.Add(tempDir);

        tempDir = new CardinalDirection();
        tempDir.vectorDir = new Vector3(0, -0.25f, 0.75f);
        tempDir.canMove = true;
        tempDir.cardinalRange = cardinalRangeDiagonal;
        cardinalDirections.Add(tempDir);

        tempDir = new CardinalDirection();
        tempDir.vectorDir = new Vector3(0, -1, 0);
        tempDir.canMove = true;
        tempDir.cardinalRange = cardinalRangeVerHor;
        cardinalDirections.Add(tempDir);

        tempDir = new CardinalDirection();
        tempDir.vectorDir = new Vector3(0, -0.25f, -0.75f);
        tempDir.canMove = true;
        tempDir.cardinalRange = cardinalRangeDiagonal;
        cardinalDirections.Add(tempDir);

        tempDir = new CardinalDirection();
        tempDir.vectorDir = new Vector3(0, 0, -1);
        tempDir.canMove = true;
        tempDir.cardinalRange = cardinalRangeVerHor;
        cardinalDirections.Add(tempDir);

        tempDir = new CardinalDirection();
        tempDir.vectorDir = new Vector3(0, 0.25f, -0.75f);
        tempDir.canMove = true;
        tempDir.cardinalRange = cardinalRangeDiagonal;
        cardinalDirections.Add(tempDir);
    }

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

    private void OnDrawGizmos()
    {
        foreach (Collider collider in victoryPoint)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(collider.bounds.center, 1);
        }
        foreach (CardinalDirection cd in cardinalDirections)
        {
            Ray r = new Ray();
            r.direction = cd.vectorDir;
            r.origin = transform.position;
            if (cd.canMove)
            {
                Gizmos.DrawRay(r);
            }
        }
    }
}
