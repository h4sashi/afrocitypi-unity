using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool isWinGame;
    public bool isGameFail;
    public GameObject hintbtn, correctCountpanel;
    public TextMeshProUGUI TxtCorrect, TxtTimer, TxtLevel, TxtLifes;

    [SerializeField] List<RectTransform> toastmsg;

    private void Awake()
    {
        instance = this;
        // LoginManager.manager.mainCanvas.SetActive(false);
    }

    public void ShowToastMsg(string msg)
    {
        RectTransform trans = toastmsg[0];
        toastmsg.RemoveAt(0);
        toastmsg.Add(trans);

        trans.GetComponent<TextMeshProUGUI>().text = msg;
        trans.GetComponent<TextMeshProUGUI>().DOFade(1f, 0.5f).SetUpdate(true);
        trans.transform.localPosition = Vector3.zero;
        trans.transform.localScale = Vector3.zero;
        trans.DOScale(1f, 0.2f).SetUpdate(true);
        trans.DOAnchorPosY(250f, 1f).SetUpdate(true);
        trans.GetComponent<TextMeshProUGUI>().DOFade(0f, 0.2f).SetDelay(0.8f).SetUpdate(true);
    }

    public int getsetHint
    {
        get { return PlayerPrefs.GetInt(PlayerPrefs.GetString("PlayerUserId") + "Hint", 1); }
        set
        {
            PlayerPrefs.SetInt(PlayerPrefs.GetString("PlayerUserId") + "Hint", value);
            UIManager.instance.UpdateHint();
        }
    }

    public int getsetCoins
    {
        get { return PlayerPrefs.GetInt(PlayerPrefs.GetString("PlayerUserId") + "Coin", 500); }
        set
        {
            PlayerPrefs.SetInt(PlayerPrefs.GetString("PlayerUserId") + "Coin", value);
            UIManager.instance.UpdateCoins();
        }
    }

    public int getsetLife
    {
        get { return PlayerPrefs.GetInt(PlayerPrefs.GetString("PlayerUserId") + "Life", 3); }
        set
        {
            PlayerPrefs.SetInt(PlayerPrefs.GetString("PlayerUserId") + "Life", value);
            if (value < 0) PlayerPrefs.SetInt(PlayerPrefs.GetString("PlayerUserId") + "Life", 0);
            TxtLifes.text = getsetLife.ToString();
        }
    }

    public void GetRewardTimebase()
    {
        ShowToastMsg("Got " + 30 + " seconds!");
        LevelManager.instance.CurlevelDetail.totalTime = 30;
        LevelManager.instance.CurlevelDetail.currentTime = 30;
        LevelManager.instance.CurlevelDetail.continueGame(true);
    }

    public void GetRewardWrongSelection()
    {
        getsetLife += 1;
        ShowToastMsg("Got " + 1 + " more lifes!");
        if (isGameFail) LevelManager.instance.CurlevelDetail.continueGame(false);
    }

    public void GetRewardShopAds()
    {
        getsetCoins += 50;
    }
}
