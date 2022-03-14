using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject naivePlayer;
    public GameObject protectorPlayer;

    public bool ifPlayerMode = true;
    public int observedAgent = 0;
    public bool activeNaivePlayer = true;

    void Start()
    {
        if (ifPlayerMode)
        {
            changeViewObjects();
            naivePlayer.GetComponent<NaivePlayer>().isActive = activeNaivePlayer;
            protectorPlayer.GetComponent<ProtectorPlayer>().isActive = !activeNaivePlayer;
            naivePlayer.GetComponent<NaiveAgent>().isActive = false;
            protectorPlayer.GetComponent<ProtectorAgent>().isActive = false;

        }
        else
        {
            changeViewObjects();
            naivePlayer.GetComponent<NaivePlayer>().isActive = false;
            protectorPlayer.GetComponent<ProtectorPlayer>().isActive = false;
            naivePlayer.GetComponent<NaiveAgent>().isActive = true;
            protectorPlayer.GetComponent<ProtectorAgent>().isActive = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && ifPlayerMode)
        {
            activeNaivePlayer = !activeNaivePlayer;
            changeViewObjects();
            naivePlayer.GetComponent<NaivePlayer>().isActive = activeNaivePlayer;
            protectorPlayer.GetComponent<ProtectorPlayer>().isActive = !activeNaivePlayer;
        }
        else if (Input.GetKeyDown(KeyCode.T) && !ifPlayerMode)
        {
            activeNaivePlayer = !activeNaivePlayer;
            changeViewObjects();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            ifPlayerMode = false;
            changeViewObjects();
            naivePlayer.GetComponent<NaivePlayer>().isActive = false;
            protectorPlayer.GetComponent<ProtectorPlayer>().isActive = false;

            naivePlayer.GetComponent<NaiveAgent>().isActive = true;
            protectorPlayer.GetComponent<ProtectorAgent>().isActive = true;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            ifPlayerMode = true;
            changeViewObjects();
            naivePlayer.GetComponent<NaivePlayer>().isActive = activeNaivePlayer;
            protectorPlayer.GetComponent<ProtectorPlayer>().isActive = !activeNaivePlayer;

            naivePlayer.GetComponent<NaiveAgent>().isActive = false;
            protectorPlayer.GetComponent<ProtectorAgent>().isActive = false;
        }




    }

    public void changeViewObjects()
    {
        if (ifPlayerMode)
        {
            if (activeNaivePlayer)
            {
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Death"))
                {
                    if (obj.GetComponent<MeshRenderer>()) obj.GetComponent<MeshRenderer>().enabled = false;
                }
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Hazard"))
                {
                    if (obj.GetComponent<MeshRenderer>()) obj.GetComponent<MeshRenderer>().enabled = false;
                }
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Victory"))
                {
                    if (obj.GetComponent<MeshRenderer>()) obj.GetComponent<MeshRenderer>().enabled = true;
                }
            }
            else
            {
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Death"))
                {
                    if (obj.GetComponent<MeshRenderer>()) obj.GetComponent<MeshRenderer>().enabled = true;
                }
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Hazard"))
                {
                    if (obj.GetComponent<MeshRenderer>()) obj.GetComponent<MeshRenderer>().enabled = true;
                }
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Victory"))
                {
                    if (obj.GetComponent<MeshRenderer>()) obj.GetComponent<MeshRenderer>().enabled = false;
                }
            }
        }
        else
        {
            if (activeNaivePlayer)
            {
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Death"))
                {
                    if (obj.GetComponent<MeshRenderer>()) obj.GetComponent<MeshRenderer>().enabled = false;
                }
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Hazard"))
                {
                    if (obj.GetComponent<MeshRenderer>()) obj.GetComponent<MeshRenderer>().enabled = false;
                }
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Victory"))
                {
                    if (obj.GetComponent<MeshRenderer>()) obj.GetComponent<MeshRenderer>().enabled = true;
                }
            }
            else
            {
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Death"))
                {
                    if (obj.GetComponent<MeshRenderer>()) obj.GetComponent<MeshRenderer>().enabled = true;
                }
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Hazard"))
                {
                    if (obj.GetComponent<MeshRenderer>()) obj.GetComponent<MeshRenderer>().enabled = true;
                }
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Victory"))
                {
                    if (obj.GetComponent<MeshRenderer>()) obj.GetComponent<MeshRenderer>().enabled = false;
                }
            }
        }
    }
}
