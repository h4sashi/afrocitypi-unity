using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LinkOpener : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    TextMeshProUGUI txt;

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(txt, Input.mousePosition, Camera.main);

        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = txt.textInfo.linkInfo[linkIndex];
            Application.OpenURL(linkInfo.GetLinkID());
        }
    }

    public void DirectLinkOpen(string s)
    {
        Application.OpenURL(s);
    }
}
