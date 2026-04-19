using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundHapticManager : MonoBehaviour
{
    private AudioSource aSource;
    public AudioClip BG, button, findMistake, win, fail, wrongSelect, timer;
    public AudioSource musicSource;
    public static SoundHapticManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
        }
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey("Sound"))
        {
            PlayerPrefs.SetInt("Sound", 1);
        }

        if (!PlayerPrefs.HasKey("Haptic"))
        {
            PlayerPrefs.SetInt("Haptic", 1);
        }
        if (!PlayerPrefs.HasKey("Music"))
        {
            PlayerPrefs.SetInt("Music", 1);
        }

        aSource = this.gameObject.AddComponent<AudioSource>();
        aSource.playOnAwake = false;
        aSource.loop = false;
        PlayMusic();
    }

    public void PlayMusic()
    {
        if (PlayerPrefs.GetInt("Music") == 1)
        {
            musicSource.mute = false;
        }
        else
        {
            musicSource.mute = true;
        }
    }

    public void playClip(AudioClip clip, float volume)
    {
        if (PlayerPrefs.GetInt("Sound") == 1)
        {
            if (aSource.isPlaying == true)
            {
                GameObject tempObj = new GameObject();
                tempObj.transform.SetParent(transform);
                AudioSource tempSource = tempObj.AddComponent<AudioSource>();
                tempSource.playOnAwake = false;
                tempSource.loop = false;
                tempSource.volume = volume;
                tempSource.clip = clip;
                tempSource.Play();
                Destroy(tempObj, tempSource.clip.length);
            }
            else
            {
                aSource.clip = clip;
                aSource.volume = volume;
                aSource.Play();
            }
        }
    }

    public void Haptic()
    {
        if (PlayerPrefs.GetInt("Haptic") == 1) VibrationController.Instance.Frequency_C();
    }

    public void ButtonClick()
    {
        playClip(button, 1);
        Haptic();
    }
}
