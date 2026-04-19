using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelections : MonoBehaviour
{
    public GameObject levelsContent;
    private void OnEnable()
    {
        int unlockLevels = LevelManager.instance.getLevel;
        for (int i = 0; i < unlockLevels; i++)
        {
            levelsContent.transform.GetChild(i).GetComponent<Button>().interactable = true;
            levelsContent.transform.GetChild(i).transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        for (int i = 0; i < levelsContent.transform.childCount; i++)
        {
            int k = i;
            levelsContent.transform.GetChild(k).gameObject.GetComponent<Button>().onClick.AddListener(() => LevelManager.instance.SelectLevel(k + 1));
            levelsContent.transform.GetChild(k).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (k + 1).ToString();
        }
    }
}
