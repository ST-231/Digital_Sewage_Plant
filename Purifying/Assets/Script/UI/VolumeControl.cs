using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundPlay : MonoBehaviour
{
    private AudioSource beginAudio;
    private AudioSource Scene1Audio;
    private Slider audioSlider;

    // Start is called before the first frame update
    void Start()
    {

        if (SceneManager.GetSceneByName("MainMap").isLoaded)
        {
            beginAudio = GameObject.FindGameObjectWithTag("MainMenu").transform.GetComponent<AudioSource>();
        }

        if (SceneManager.GetSceneByName("SewagePlant").isLoaded)
        {
            Scene1Audio = GameObject.FindGameObjectWithTag("SettingUI").transform.GetComponent<AudioSource>();
        }

        audioSlider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        volume();
    }

    public void volume()
    {

        if (beginAudio != null)
        {
            beginAudio.volume = audioSlider.value;
        }
        if (Scene1Audio != null)
        {
            Scene1Audio.volume = audioSlider.value;
        }
    }

}
