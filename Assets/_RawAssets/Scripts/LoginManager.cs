using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public GameObject loginOption;
    public GameObject loadingBar;
    public TextMeshProUGUI feedbackText;
    private float loadingTimer;

    private void Start()
    {
        // Disable loginOption initially (if not already)
        if (loginOption != null)
        {
            loginOption.SetActive(false);
        }
        
        // Start the timer coroutine
        StartCoroutine(DisplayLoginOptionAfterDelay(5f));
    }

    private IEnumerator DisplayLoginOptionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (loginOption != null)
        {
            loginOption.SetActive(true);
        }
        loadingBar.SetActive(false);
    }
}
