using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TimeDisplay : MonoBehaviour
{
    //public TextMeshPro _displaySeconds;
    //public TextMeshPro _displayMinutes;
    public GameObject _displaySeconds;
    public GameObject _displayMinutes;
    public double _time = 0;

    public bool pause = false;

    private void Start()
    {
        pause = false;
    }

    CacheIntString cacheSeconds = new CacheIntString(
        (seconds) => seconds % 60, //describe how seconds (key) are translated to useful value (hash)
        (second) => second.ToString("00") //you describe how string is built based on value (hash)
        , 0, 59, 1 //initialization range and step, so cache will be warmed up and ready
    );
    CacheIntString cacheMinutes = new CacheIntString(
        (seconds) => seconds / 60 % 60, // this translates input seconds to minutes
        (minute) => minute.ToString("00") // this translates minute to string
        , 0, 60, 60 //minutes needs a step of 60 seconds
    );
    void Update()
    {
        if (!pause) {
            _time = Time.timeSinceLevelLoad;
            int seconds = Mathf.FloorToInt((float)_time);
            _displaySeconds.GetComponent<TMPro.TextMeshProUGUI>().text = cacheSeconds[seconds];
            //Debug.Log(cacheSeconds[seconds]);
            _displayMinutes.GetComponent<TMPro.TextMeshProUGUI>().text = cacheMinutes[seconds];
            //Debug.Log(cacheMinutes[seconds]);
        }
    }
}
