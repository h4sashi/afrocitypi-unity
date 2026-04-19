using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMistake : MonoBehaviour
{
    public Vector3 mousePos;
    public int Count = 0;
    [Range(40, 100)]
    public float radius = 40;
    [Range(100, 200)]
    public float ImageSize;

    private void Start()
    {
        radius = 40;
        for (int i = 0; i < 5; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        Count = 0;
    }

    private void Update()
    {

        transform.GetChild(Count).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(ImageSize, ImageSize);
        transform.GetChild(Count).GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(ImageSize, ImageSize);
        if (transform.GetChild(Count).GetChild(0).GetComponent<CircleCollider2D>())
        {
            transform.GetChild(Count).GetChild(0).GetComponent<CircleCollider2D>().radius = radius;
            transform.GetChild(Count).GetChild(1).GetComponent<CircleCollider2D>().radius = radius;
        }



        if (Input.GetMouseButtonDown(0) && Count < transform.childCount)
        {
            mousePos = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);

            transform.GetChild(Count).gameObject.SetActive(true);
            transform.GetChild(Count).GetChild(0).transform.localPosition = mousePos;

            transform.GetChild(Count).gameObject.AddComponent<Anchor>();

            //transform.GetChild(Count).GetChild(0).GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            //transform.GetChild(Count).GetChild(1).GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Count++;
        }
    }

}
