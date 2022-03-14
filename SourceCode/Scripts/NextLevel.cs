using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{

    public GameObject timeDisplay;
    public GameObject button;
    public GameObject naivePlayer;

    void Start()
    {
        button.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (timeDisplay.GetComponent<TimeDisplay>().pause)
        {
            naivePlayer.GetComponent<NaiveAgent>().isActive = false;
            naivePlayer.GetComponent<NaiveAgent>().Idle();
            button.SetActive(true);
        }
    }

    public void nextLevel(string Level)
    {
        SceneManager.LoadScene(Level);
    }
}
