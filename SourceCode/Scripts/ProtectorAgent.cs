using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectorAgent : MonoBehaviour
{
    ///////////////////////////////////////////
    /// Movement Variables
    ///////////////////////////////////////////
    public Rigidbody rb;
    public float moveSpeed = 10;
    public float jumpForce = 2;
    public int jumpsLeft = 1;
    public int maxJumpCount = 1;
    public bool isActive = false;
    public bool isColliding = false;
    public bool shouldJump = false;

    ///////////////////////////////////////////
    /// Naive Variables
    ///////////////////////////////////////////
    public GameObject naiveAgent;
    public GameObject closestEnemy;
    public bool avoidNaive = false;
    public Vector3 naiveDirection;

    ///////////////////////////////////////////
    /// World Variables
    ///////////////////////////////////////////

    public List<Collider> hazardsAndDeaths = new List<Collider>();
    public List<Collider> wallsAndFloor = new List<Collider>();
    public LayerMask whatIsNaive;

    ///////////////////////////////////////////
    /// RayCast Variables
    ///////////////////////////////////////////

    public float naiveDangerDist = 5;
    public int naiveDangerValue = 0;
    public float cardinalRangeVerHor = 5;
    public float cardinalRangeDiagonal = 5;
    public List<CardinalDirection> cardinalDirections = new List<CardinalDirection>();
    public LayerMask whatIsGround;
    public float isGroundedRayLength = 0.05f;

    public struct CardinalDirection
    {
        public Vector3 vectorDir;
        public RaycastHit closestHitPoint;
        public bool canMove;
        public float cardinalRange;
    }


    ///////////////////////////////////////////
    /// Setup
    ///////////////////////////////////////////

    void Start()
    {
        //Getting all the world objects: Hazards and walls
        var allObjectsWithTagHazard = GameObject.FindGameObjectsWithTag("Hazard");
        var allObjectsWithTagDeath = GameObject.FindGameObjectsWithTag("Death");
        var allObjectsWithTagWall = GameObject.FindGameObjectsWithTag("Wall");

        //Saving the colliders of the world objects
        foreach (GameObject hazards in allObjectsWithTagHazard)
        {
            hazardsAndDeaths.Add(hazards.GetComponent<Collider>());
        }
        foreach (GameObject death in allObjectsWithTagDeath)
        {
            hazardsAndDeaths.Add(death.GetComponent<Collider>());
        }
        foreach (GameObject wall in allObjectsWithTagWall)
        {
            wallsAndFloor.Add(wall.GetComponent<Collider>());
        }

        //Defining the Raycasts
        AddCardinalDirs();
        RayCastCardinalDirs();
    }

    ///////////////////////////////////////////
    /// Run
    ///////////////////////////////////////////
    void FixedUpdate()
    {
        //Check if agent is on the ground: resets the jump counter
        if (isGrounded)
        {
            jumpsLeft = maxJumpCount;
        }
        
        //Defines the directions the agent can move to
        RayCastCardinalDirs();

        //Chooses the aproppriate action depending on what the agent perceives and wants to achieve
        Deliberate();
    }

    ///////////////////////////////////////////
    /// Function responsible for the action decision. By taking the components of world and the naive agent, the protector agent 
    /// decides what's the best action to execute
    ///////////////////////////////////////////
    public void Deliberate()
    {
        //Sees in a number of directions around the Naive agent and calculates the amount of dangers around it and then sends that number to it
        PercieveNaiveDangerLevel();

        //If the naive agent is in the way, the protector agent tries its best to avoid him (since the agents collide, therefore they can push each other)
        if( avoidNaive) AvoidNaive();

        

        //Sees if there is any danger around the naive agent
        if (naiveDangerValue >= 1)
        {
            //Checks what is the closest danger to the naive agent and acts accordingly
            switch (closestEnemy.GetComponent<EnemyClassifier>().enemyTag)
            {
                case "Spike":
                    addressSpike();
                    break;
                case "FallBlock":
                    addressFallBlock();
                    break;
                case "Saw":
                    addressSaw();
                    break;

                default:
                    Debug.Log("Shouldn't happen"); 
                    break;

            }
        }
        else if (distanceToNaive() >= 10)
        {
            OrientedMoveWithRayCast(naiveAgent.transform.position - transform.position);
        }
        if (shouldJump) Jump();

    }



    ///////////////////////////////////////////
    /// Protector Agent Actions
    ///////////////////////////////////////////

    // -----------------------------
    // Just moves the Agent in the direction of the given vector (Can't handle jump)
    // -----------------------------
    public void OrientedMove(Vector3 dir)
    {
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, Vector3.Normalize(dir).z * moveSpeed);
    }

    // -----------------------------
    // Tries to Move the Agent in the direction of the given vector, choosing the best possible path according to the raycasts (Can handle jump)
    // -----------------------------
    public void OrientedMoveWithRayCast(Vector3 dir)
    {
        Vector3 bestDirection = new Vector3(0, 0, 0);
        float angleBetweenDirs = 180;

        //cd.VectorDir is one of the possible vectors, dir is the direction given, the cd.VectorDir with the smallest angle
        //between it and dir is choosen
        foreach (CardinalDirection cd in cardinalDirections)
        {
            if (Vector3.Angle(cd.vectorDir, dir) < angleBetweenDirs && cd.canMove)
            {
                bestDirection = cd.vectorDir;
                angleBetweenDirs = Vector3.Angle(cd.vectorDir, dir);
            }
        }

        //Moves Forward
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, Vector3.Normalize(bestDirection).z * moveSpeed);

        //Handles jumping if needed
        if (bestDirection.y > 0 && jumpsLeft > 0)
        {
            rb.velocity = rb.velocity + Vector3.up * jumpForce;
            jumpsLeft--;
        }
    }

    // -----------------------------
    // Tries to jump over the Naive agent in order to not push it
    // -----------------------------
    public void AvoidNaive()
    {
        if (jumpsLeft > 0)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpsLeft--;
        }
    }

    // -----------------------------
    // If the closest hazard/danger to the Naive agent is a Saw, then the Protector agent takes the appropriate
    // steps to better protect the naive agent. Does this by going on top of the saw and telling the naive
    // agent to jump over him
    // -----------------------------
    public void addressSaw()
    {
        //Makes sure the agent is with its standard shape (1x1 square)
        changeShape(1);

        //Checks if the saw is above the naive agent, if it is then informs the naive agent to not jump 
        if (closestEnemy.transform.position.y > naiveAgent.transform.position.y)
        {
            naiveAgent.GetComponent<NaiveAgent>().canJump = false;
            naiveAgent.GetComponent<NaiveAgent>().DangerValue = 0;

            closestEnemy.GetComponent<EnemyClassifier>().isSaw = false;
            closestEnemy.GetComponent<EnemyClassifier>().isAdressed = true;
            closestEnemy.GetComponent<EnemyClassifier>().reEvaluate();
        }

        else {
            naiveAgent.GetComponent<NaiveAgent>().canJump = true;

            //If the agent is not colliding with the hazard, then the agent tries to move to the hazards position
            if (!closestEnemy.GetComponent<EnemyClassifier>().col)
            {
                Vector3 dir = (closestEnemy.gameObject.transform.position + transform.position) / 2 - transform.position;
                OrientedMove(dir);
            }
            else
            {
                //Checks if the naive agent has passed the hazard or not
                if (Vector3.Angle(naiveDirection, closestEnemy.gameObject.transform.position - naiveAgent.transform.position) < 90)
                {
                    //Checks if the protector agent is on top of the saw (or at least the ground)
                    if (-0.5f < rb.velocity.y && rb.velocity.y < 0.5)
                    {
                        //If the protector agent is on contact with the hazard then he informs the naive agent that the danger has
                        //been dealt with and gives him its next action
                        if (closestEnemy.GetComponent<EnemyClassifier>().col)
                        {
                            naiveAgent.GetComponent<NaiveAgent>().DangerValue = 0;
                            naiveAgent.GetComponent<NaiveAgent>().shouldJump = true;
                        }
                    }
                }
                else
                {
                    //If the naive agent has passed the hazard then the protector agent updates its percepion on the hazard
                    if (closestEnemy.GetComponent<EnemyClassifier>().col)
                    {
                        changeShape(1);
                        closestEnemy.GetComponent<EnemyClassifier>().isSaw = false;
                        closestEnemy.GetComponent<EnemyClassifier>().isAdressed = true;
                        closestEnemy.GetComponent<EnemyClassifier>().reEvaluate();

                        if (IsCBetweenAB(transform.position, naiveAgent.transform.position, closestEnemy.transform.position))
                        {
                            jumpsLeft = maxJumpCount;
                            shouldJump = true;
                        }
                    }
                }
            }
        }

        //Checks if the agent collided with the target danger
        
    }

    // -----------------------------
    // If the closest hazard/danger to the Naive agent is a Spike, then the Protector agent takes the appropriate
    // steps to better protect the naive agent. Does this by changing shape,  going on top of the spike and telling the naive
    // agent to jump over him
    // -----------------------------
    public void addressSpike()
    {
        naiveAgent.GetComponent<NaiveAgent>().canJump = true;

        //Checks if the agent is close to the falling block so it can transform
        if (System.Math.Abs(closestEnemy.gameObject.transform.position.z - transform.position.z) <= 5)
        {
            //Changes the agent to its horizontal shape (1x3 rectangle)
            changeShape(3);
        }

        //If the agent is not colliding with the hazard, then the agent tries to move to the hazards position
        if (!closestEnemy.GetComponent<EnemyClassifier>().col)
        {
            Vector3 dir = (closestEnemy.gameObject.transform.position + transform.position) / 2 - transform.position;
            OrientedMoveWithRayCast(dir);
        }
        else
        {
            //Checks if the naive agent has passed the hazard or not
            if (Vector3.Angle(naiveDirection, closestEnemy.gameObject.transform.position - naiveAgent.transform.position) < 90)
            {
                //Checks if the protector agent is on top of the spike (or at leats the ground)
                if (-0.5f < rb.velocity.y && rb.velocity.y < 0.5)
                {
                    //If the protector agent is on contact with the hazard then he informs the naive agent that the danger has
                    //been dealt with and gives him its next action
                    if (closestEnemy.GetComponent<EnemyClassifier>().col)
                    {
                        naiveAgent.GetComponent<NaiveAgent>().DangerValue = 0;
                        naiveAgent.GetComponent<NaiveAgent>().shouldJump = true;
                    }
                }
            }
            else
            {
                //If the naive agent has passed the hazard then the protector agent updates its percepion on the hazard
                if (closestEnemy.GetComponent<EnemyClassifier>().col)
                {
                    changeShape(1);
                    closestEnemy.GetComponent<EnemyClassifier>().isSaw = false;
                    closestEnemy.GetComponent<EnemyClassifier>().isAdressed = true;
                    closestEnemy.GetComponent<EnemyClassifier>().reEvaluate();

                    if (IsCBetweenAB(transform.position, naiveAgent.transform.position, closestEnemy.transform.position))
                    {
                        jumpsLeft = maxJumpCount;
                        shouldJump = true;
                    }
                }
            }
        }
    }

    // -----------------------------
    // If the closest hazard/danger to the Naive agent is a Falling Block, then the Protector agent takes the appropriate
    // steps to better protect the naive agent. Does this by changing shape, waiting for the falling block to reach and appropriate
    // height and then touching it, causing it to stop and oppening way for the naive agent
    // -----------------------------
    public void addressFallBlock()
    {
        naiveAgent.GetComponent<NaiveAgent>().canJump = true;

        //Checks if the agent is close to the falling block so it can transform
        if (System.Math.Abs(closestEnemy.gameObject.transform.position.z - transform.position.z) <= 5f)
        {
            //Changes the agent to its horizontal shape (3x1 rectangle)
            changeShape(2);
        }


        //Checks the height of the block and if the height has reached a certain threshold, moves to under the falling block
        if (closestEnemy.gameObject.transform.position.y - closestEnemy.GetComponent<FallingBlock>().botPos >= 2.5)
        {

            Vector3 directVector = new Vector3(0, closestEnemy.gameObject.transform.position.y - transform.position.y, 
                                                    closestEnemy.gameObject.transform.position.z - transform.position.z);
            directVector = new Vector3(0, 0, closestEnemy.gameObject.transform.position.z - transform.position.z);
            OrientedMoveWithRayCast(directVector);
        } else if (System.Math.Abs( closestEnemy.gameObject.transform.position.z - transform.position.z) >= 5)
        {
            Vector3 horinzontalVector = new Vector3(0, 0, closestEnemy.gameObject.transform.position.z - transform.position.z);
            OrientedMoveWithRayCast(horinzontalVector);
        }
        else
        {
            Idle();
        }

        //If the protector agent has collied with the block, then it updates its percepion on the hazard and changes its shape
        if (closestEnemy.GetComponent<FallingBlock>().col)
        {
            Vector3 getBehindVector = new Vector3(0, 0, (naiveAgent.transform.position.z - 2f) - transform.position.z);
            if (Vector3.Angle(naiveDirection, getBehindVector) > 90)
            {
                OrientedMoveWithRayCast(getBehindVector);
            }

            changeShape(1);
            closestEnemy.GetComponent<EnemyClassifier>().isFallBlock = false;
            closestEnemy.GetComponent<EnemyClassifier>().isAdressed = true;
            closestEnemy.GetComponent<EnemyClassifier>().reEvaluate();
        }
        
    }

    // -----------------------------
    // Changes the shape of the protector agent
    // -----------------------------
    public void changeShape(int shapeNum)
    {
        Vector3 scaleChange = new Vector3(1, 1, 1);
        switch (shapeNum)
        {
            case 1:
                //1x1 square
                scaleChange = new Vector3(1, 1, 1);
                this.gameObject.transform.localScale = scaleChange;
                break;
            case 2:
                //3x1 rectangle
                scaleChange = new Vector3(1, 3, 1);
                if(checkOuterBounds(2)) this.gameObject.transform.localScale = scaleChange;
                break;
            case 3:
                //1x3 rectangle
                scaleChange = new Vector3(1, 1, 3);
                if (checkOuterBounds(1))  this.gameObject.transform.localScale = scaleChange;
                break;
            default:
                scaleChange = new Vector3(1, 1, 1);
                this.gameObject.transform.localScale = scaleChange;
                break;
        }
    }

    // -----------------------------
    // The protector agent jumps
    // -----------------------------
    public void Jump()
    {
        //Checks if the agent can still jump, or if it's safe to jump (according to the protector agent)
        if (jumpsLeft > 0)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpsLeft--;
        }
        shouldJump = false;
    }

    public void Idle()
    {
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
    }

    ///////////////////////////////////////////
    /// Communication - Recieving
    ///////////////////////////////////////////



    ///////////////////////////////////////////
    /// Protector Agent Sensors
    ///////////////////////////////////////////


    // -----------------------------
    // Perceives the amount of dangers that surrounds the naive agent 
    // -----------------------------
    public void PercieveNaiveDangerLevel()
    {
        float closestDistance = 1000;
        naiveDangerValue = 0;

        foreach (Collider col in hazardsAndDeaths)
        {
            if(col.gameObject.tag == "Death")
            {
                if (!col.gameObject.GetComponent<EnemyClassifier>().isAdressed)
                {
                    //Checks if the collision detected is considered an hazard
                    if (RaycastDanger(col))
                    {
                        if (Vector3.Magnitude(col.bounds.center - naiveAgent.transform.position) < naiveDangerDist * naiveDangerDist)
                        {
                            if (Vector3.Distance(col.bounds.center, naiveAgent.transform.position) < closestDistance)
                            {
                                closestEnemy = col.gameObject;
                            }
                            naiveDangerValue++;
                        }
                    }
                }
            }
        }
        naiveAgent.GetComponent<NaiveAgent>().DangerValue = naiveDangerValue;
    }

    // -----------------------------
    // Checks if the collision detected is considered an hazard. Does this by checking all the bounds of the hazard collider
    // -----------------------------
    public bool RaycastDanger(Collider col)
    {
        RaycastHit hit;
        if (Physics.Raycast(naiveAgent.transform.position, col.bounds.center - naiveAgent.transform.position, out hit, 100000)){
            if (hit.transform.tag == "Hazard" || hit.transform.tag == "Death")
                return true;
        }
        if (Physics.Raycast(naiveAgent.transform.position, col.bounds.min - naiveAgent.transform.position, out hit, 100000))
        {
            if (hit.transform.tag == "Hazard" || hit.transform.tag == "Death")
                return true;
        }
        if (Physics.Raycast(naiveAgent.transform.position, col.bounds.max - naiveAgent.transform.position, out hit, 100000))
        {
            if (hit.transform.tag == "Hazard" || hit.transform.tag == "Death")
                return true;
        }
        return false;
    }

    // -----------------------------
    // Registers the directions that the protector agent can take. Considering this can't pass through walls or hazards or the naive agent itself
    // -----------------------------
    public void RayCastCardinalDirs()
    {
        for (int i = 0; i < cardinalDirections.Count; i++)
        {
            CardinalDirection tempDir = new CardinalDirection();
            tempDir.vectorDir = cardinalDirections[i].vectorDir;
            tempDir.cardinalRange = cardinalDirections[i].cardinalRange;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, tempDir.vectorDir, out hit, tempDir.cardinalRange))
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

    public bool checkOuterBounds(int shape)
    {
        //Check if Vertical shape
        if(shape == 3)
        {
            RaycastHit hit_right;
            if (Physics.Raycast(transform.position, new Vector3(0,0,1) , out hit_right, 1.5f))
            {
                return false;
            }

            RaycastHit hit_left;
            if (Physics.Raycast(transform.position, new Vector3(0, 0, -1), out hit_left, 1.5f))
            {
                return false;
            }
        }
        //Check if horizontal shape
        else if (shape == 2)
        {
            RaycastHit hit_up;
            if (Physics.Raycast(transform.position, new Vector3(0, 1, 0), out hit_up, 1.5f))
            {
                return false;
            }

            RaycastHit hit_bot;
            if (Physics.Raycast(transform.position, new Vector3(0, -1, 0), out hit_bot, 1.5f))
            {
                return false;
            }
        }
        return true;
    }

    public float distanceToNaive()
    {
        float distance = Vector3.Distance(naiveAgent.transform.position, transform.position);
        return distance;
    }


    ///////////////////////////////////////////
    /// Auxiliary Functions
    ///////////////////////////////////////////

    // -----------------------------
    // Checks if the protector agent is on the ground
    // -----------------------------
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

    // -----------------------------
    // Initializes all 8 percetion raycasts
    // -----------------------------
    public void AddCardinalDirs()
    {
        CardinalDirection tempDir = new CardinalDirection();
        tempDir.vectorDir = new Vector3(0, 1, 0);
        tempDir.canMove = true;
        tempDir.cardinalRange = cardinalRangeVerHor;
        cardinalDirections.Add(tempDir);

        tempDir = new CardinalDirection();
        tempDir.vectorDir = new Vector3(0, 0.5f, 0.5f);
        tempDir.canMove = true;
        tempDir.cardinalRange = cardinalRangeDiagonal;
        cardinalDirections.Add(tempDir);

        tempDir = new CardinalDirection();
        tempDir.vectorDir = new Vector3(0, 0, 1);
        tempDir.canMove = true;
        tempDir.cardinalRange = cardinalRangeVerHor;
        cardinalDirections.Add(tempDir);

        tempDir = new CardinalDirection();
        tempDir.vectorDir = new Vector3(0, -0.5f, 0.5f);
        tempDir.canMove = true;
        tempDir.cardinalRange = cardinalRangeDiagonal;
        cardinalDirections.Add(tempDir);

        tempDir = new CardinalDirection();
        tempDir.vectorDir = new Vector3(0, -1, 0);
        tempDir.canMove = true;
        tempDir.cardinalRange = cardinalRangeVerHor;
        cardinalDirections.Add(tempDir);

        tempDir = new CardinalDirection();
        tempDir.vectorDir = new Vector3(0, -0.5f, -0.5f);
        tempDir.canMove = true;
        tempDir.cardinalRange = cardinalRangeDiagonal;
        cardinalDirections.Add(tempDir);

        tempDir = new CardinalDirection();
        tempDir.vectorDir = new Vector3(0, 0, -1);
        tempDir.canMove = true;
        tempDir.cardinalRange = cardinalRangeVerHor;
        cardinalDirections.Add(tempDir);

        tempDir = new CardinalDirection();
        tempDir.vectorDir = new Vector3(0, 0.5f, -0.5f);
        tempDir.canMove = true;
        tempDir.cardinalRange = cardinalRangeDiagonal;
        cardinalDirections.Add(tempDir);
    }

    // -----------------------------
    // Checks if a given vector is between two other given vectors
    // -----------------------------
    bool IsCBetweenAB(Vector3 A, Vector3 B, Vector3 C)
    {
        return Vector3.Dot((B - A).normalized, (C - B).normalized) < 0f && Vector3.Dot((A - B).normalized, (C - A).normalized) < 0f;
    }

    // -----------------------------
    // Draws the gizmos that allow to see the colliders of the hazards and walls and the possible raycasts
    // -----------------------------
    private void OnDrawGizmos()
    {
        foreach (Collider collider in hazardsAndDeaths)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(collider.bounds.center, 1);
        }
        foreach (Collider collider in wallsAndFloor)
        {
            Gizmos.color = Color.red;
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
