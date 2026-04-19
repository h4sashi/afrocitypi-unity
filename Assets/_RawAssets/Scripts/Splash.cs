using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Image>().DOFade(1, 1.5f).OnComplete(() => { SceneManager.LoadScene("Login"); });
    }
}
