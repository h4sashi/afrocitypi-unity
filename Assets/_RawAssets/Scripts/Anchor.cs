using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Anchor : MonoBehaviour
{
    RectTransform rec1, rec2;

    private void Start()
    {
        rec1 = transform.GetChild(0).GetComponent<RectTransform>();
        rec2 = transform.GetChild(1).GetComponent<RectTransform>();

        Vector2 pos = (transform.GetChild(0).localPosition + (HeightWidth() / 2)) / new Vector2(Screen.width, Screen.height);
        rec1.anchorMin = pos;
        rec1.anchorMax = pos;
        rec1.anchoredPosition = Vector3.zero;

        rec2.anchorMin = rec1.anchorMin + new Vector2(0, -0.5f);
        rec2.anchorMax = rec1.anchorMax + new Vector2(0, -0.5f);
        rec2.anchoredPosition = Vector3.zero;

        Destroy(this);
    }

    private void Update()
    {
        //rec = GetComponent<RectTransform>();
        //pos = (transform.localPosition + (HeightWidth() / 2)) / new Vector2(Screen.width, Screen.height);
        //rec.anchorMin = pos;
        //rec.anchorMax = pos;
        //rec.anchoredPosition = Vector3.zero;

    }

    Vector3 HeightWidth()
    {
        return new Vector3(Screen.width, Screen.height);
    }
}
