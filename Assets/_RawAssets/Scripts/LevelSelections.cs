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
        // First apply whatever is locally cached
        ApplyUnlockedLevels(LevelManager.instance.getLevel);

        // Then sync from PlayFab in case player signed in on a new device
        PlayFabManager.Instance.LoadPlayerLevelProgress(() =>
        {
            ApplyUnlockedLevels(LevelManager.instance.getLevel);
        });
    }

    private void Start()
    {
        for (int i = 0; i < levelsContent.transform.childCount; i++)
        {
            int k = i;
            levelsContent.transform.GetChild(k).GetComponent<Button>()
                .onClick.AddListener(() => LevelManager.instance.SelectLevel(k + 1));
            levelsContent.transform.GetChild(k).transform.GetChild(0)
                .GetComponent<TextMeshProUGUI>().text = (k + 1).ToString();
        }
    }

    private void ApplyUnlockedLevels(int unlockedCount)
    {
        for (int i = 0; i < levelsContent.transform.childCount; i++)
        {
            bool isUnlocked = i < unlockedCount;
            levelsContent.transform.GetChild(i).GetComponent<Button>().interactable = isUnlocked;
            levelsContent.transform.GetChild(i).GetChild(1).gameObject.SetActive(!isUnlocked);
        }
    }
}