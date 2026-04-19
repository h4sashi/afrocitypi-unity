using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSet : MonoBehaviour
{
    public SpriteRenderer bg;

    public List<Sprite> bgd = new List<Sprite>();
    private void Start()
    {
        Fitcam();
    }
    void Fitcam()
    {
        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = bg.bounds.size.x / bg.bounds.size.y;

        if (screenRatio >= targetRatio)
        {
            transform.GetComponent<Camera>().orthographicSize = bg.bounds.size.y / 2;
        }
        else
        {
            float differenceInSize = targetRatio / screenRatio;
            transform.GetComponent<Camera>().orthographicSize = bg.bounds.size.y / 2 * differenceInSize;
        }
    }
}
