using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetVolumeSlider : MonoBehaviour
{
    [SerializeField] Slider VolumeSlider;
    [SerializeField] PlayerController Player;
    [SerializeField] Text Percentage;
    public int intValue;
    [SerializeField] AudioListener Listener;

    // Start is called before the first frame update
    void Start()
    {
        VolumeSlider.value = Player.VolumeValue;
    }

    // Update is called once per frame
    void Update()
    {

        Player.VolumeValue = VolumeSlider.value;
        intValue = (int)(VolumeSlider.value*100);
        Percentage.text = intValue.ToString();

    }
}
