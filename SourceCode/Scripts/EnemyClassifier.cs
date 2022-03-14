using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClassifier : MonoBehaviour
{
    // Start is called before the first frame update

    public string enemyTag;

    public bool isAdressed = false;
    public bool isSpike = false;
    public bool isFallBlock = false;
    public bool isBullet = false;
    public bool isSaw = false;

    public bool col = false;

    void Start()
    {
        if (isSpike)
        {
            enemyTag = "Spike";
        } else if(isFallBlock)
        {
            enemyTag = "FallBlock";
        } else if (isBullet)
        {
            enemyTag = "Bullet";
        } else if (isSaw)
        {
            enemyTag = "Saw";
        } else if(isAdressed)
        {
            enemyTag = "EnemyAdressed";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void reEvaluate()
    {
        if (isSpike)
        {
            enemyTag = "Spike";
        }
        else if (isFallBlock)
        {
            enemyTag = "FallBlock";
        }
        else if (isBullet)
        {
            enemyTag = "Bullet";
        }
        else if (isSaw)
        {
            enemyTag = "Saw";
        }
        else if (isAdressed)
        {
            enemyTag = "EnemyAdressed";
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        col = true;
    }
}
