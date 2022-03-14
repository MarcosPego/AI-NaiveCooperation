using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine; 
using TMPro;

public class AlphaSettings : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider slider;
    public bool sliderChanged = false;
    public int alphaValue = 0;
    public GameObject text;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (sliderChanged)
        {
            sliderChanged = false;
            alphaValue = (int) slider.value;
            text.GetComponent<TextMeshProUGUI>().text =  alphaValue.ToString();
            PlayerPrefs.SetInt("alpha", alphaValue);
        }
    }

    public void sliderWasChanged()
    {
        sliderChanged = true;
    }
}
