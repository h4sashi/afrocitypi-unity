using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Sprite onSprite, offSprite;
    public Image sound, music;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("Sound") == 1)
        {
            sound.sprite = onSprite;
        }
        else
        {
            sound.sprite = offSprite;

        }

        if (PlayerPrefs.GetInt("Music") == 1)
        {
            music.sprite = onSprite;
        }
        else
        {
            music.sprite = offSprite;
        }
    }


    public void ChangeMusicSound(string name)
    {
        if (name == "Sound")
        {
            if (PlayerPrefs.GetInt("Sound") == 1)
            {
                PlayerPrefs.SetInt("Sound", 0);
                sound.sprite = offSprite;
            }
            else
            {
                PlayerPrefs.SetInt("Sound", 1);
                sound.sprite = onSprite;
            }
        }
        else if (name == "Music")
        {
            if (PlayerPrefs.GetInt("Music") == 1)
            {
                PlayerPrefs.SetInt("Music", 0);
                music.sprite = offSprite;
            }
            else
            {
                PlayerPrefs.SetInt("Music", 1);
                music.sprite = onSprite;
            }
            SoundHapticManager.Instance.PlayMusic();
        }
    }

    public void OnRateUs()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=" + Application.identifier);
    }

    public void OnPrivacyPolicy()
    {
        Application.OpenURL("https://doc-hosting.flycricket.io/afrocity-pi-privacy-policy/99865b9c-1dfe-4ef7-9bfd-256fcaf997cf/privacy");
    }

    public void OnTermUse()
    {
        Application.OpenURL("https://doc-hosting.flycricket.io/afrocity-pi-terms-of-use/f0abfcc6-d29d-4ecf-9bcd-aa6cd2b58e24/terms");
    }

    public void OnHelpAndSupport()
    {
        Application.OpenURL("mailto:games@zannoza.com");
    }
}
