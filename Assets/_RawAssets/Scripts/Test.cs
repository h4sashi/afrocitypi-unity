using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(FormatTime(1));
    }
    public string FormatTime(float time)
    {
        int minutes = (int)time / 60000;
        int seconds = (int)time / 1000 - 60 * minutes;
        int milliseconds = (int)time - minutes * 60000 - 1000 * seconds;
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
}
