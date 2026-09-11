using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SensitivitySliderScript : MonoBehaviour
{
    [SerializeField] Slider SensitivitySlider;
    [SerializeField] PlayerController Player;
    [SerializeField] Text Percentage;
    public int intValue;

    // Start is called before the first frame update
    void Start()
    {
        SensitivitySlider.value = Player.mouseSensitivity;
    }

    // Update is called once per frame
    void Update()
    {

        Player.mouseSensitivity = SensitivitySlider.value;
        intValue = (int)(SensitivitySlider.value * 100);
        Percentage.text = intValue.ToString();

    }
}
