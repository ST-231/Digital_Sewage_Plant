using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class MouseSensitive : MonoBehaviour
{
    private Slider audioSlider;

    void Start()
    {
        audioSlider = GetComponent<Slider>();
    }

    void Update()
    {
        if (CharacterMove.instance != null)
        {
            CharacterMove.instance.mouseSensetivity = audioSlider.value * 10;
        }
    }
}
