using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float TotalTimeIn_Second = 120;
    public bool timerIsRunning = false;
    public Text timeText;

    public bool StopTimer;

    void Update()
    {

        if (StopTimer == true)
        {
            return;
        }

        if (timerIsRunning)
        {

            TotalTimeIn_Second -= Time.deltaTime;
            DisplayTime(TotalTimeIn_Second);
        }
    }
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        Debug.Log(string.Format("{0:00}:{1:00}", minutes, seconds));
    }

}