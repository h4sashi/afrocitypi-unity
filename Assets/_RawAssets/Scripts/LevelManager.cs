using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public int hinttotal, hintused;
    public TextMeshProUGUI txthint;
    public RectTransform UiCanvas;
    public Transform LevelContainer;
    public LevelDetails CurlevelDetail;
    public List<GameObject> levelsList = new List<GameObject>();
    public GameObject Mistackes;
    public Sprite hintSprite, greenColor;
    public RuntimeAnimatorController hintAnimation;
    public ParticleSystem heartParticle;
    public bool isHintClickable = true;

    [Header("Play Msg")]
    [SerializeField] Image msgBg;
    [SerializeField] RectTransform msgPopup;

    public List<int> Index;

    private void Awake()
    {
        instance = this;
    }

    int playinglevel;
    private void Start()
    {
        UIManager.instance.UpdateHint();
        playinglevel = getLevel;
    }

    public void OnClickHint()
    {
        if (!isHintClickable && (GameManager.instance.getsetHint + PlayerPrefs.GetInt("BonusHintOf" + PlayerPrefs.GetInt("currentLevel"))) > 0) return;
        if ((GameManager.instance.getsetHint + PlayerPrefs.GetInt("BonusHintOf" + PlayerPrefs.GetInt("currentLevel"))) <= 0)
        {
            UIManager.instance.OnClick_Shop("Hint");
            return;
        }

        if (PlayerPrefs.GetInt("BonusHintOf" + PlayerPrefs.GetInt("currentLevel")) == 1)
        {
            PlayerPrefs.SetInt("BonusHintOf" + PlayerPrefs.GetInt("currentLevel"), 0);
        }
        else
        {
            GameManager.instance.getsetHint--;
            UIManager.instance.UpdateHint();
        }

        if (!GameManager.instance.isWinGame && !GameManager.instance.isGameFail)
        {
            isHintClickable = false;
            int n = Index[Random.Range(0, Index.Count)];
            int temp;
            if (Random.Range(0, 100) % 2 == 0) temp = 0;
            else temp = 1;

            if (!Mistackes.transform.GetChild(n).GetChild(temp).GetComponent<Animator>())
                Mistackes.transform.GetChild(n).GetChild(temp).gameObject.AddComponent<Animator>().runtimeAnimatorController = hintAnimation;

            Mistackes.transform.GetChild(n).GetChild(temp).GetComponent<Image>().sprite = hintSprite;
            Mistackes.transform.GetChild(n).GetChild(temp).GetComponent<Image>().enabled = true;

            hintused++;
            txthint.text = "Hint(" + (GameManager.instance.getsetHint + PlayerPrefs.GetInt("BonusHintOf" + PlayerPrefs.GetInt("currentLevel"))) + ")";
        }

    }

    public void SelectLevel(int level)
    {
        playinglevel = level;
        LoadLevel();
    }

    public void NextLevel()
    {
        playinglevel++;
        if (playinglevel > 10) playinglevel = 1;
        LoadLevel();
    }

    public void LoadLevel()
    {
        GameManager.instance.TxtLifes.text = GameManager.instance.getsetLife.ToString();
        GameManager.instance.TxtLevel.text = "LEVEL " + playinglevel;
        for (int i = 0; i < LevelContainer.childCount; i++) Destroy(LevelContainer.GetChild(i).gameObject);
        GameObject levelObj = Instantiate(levelsList[playinglevel - 1], LevelContainer);
        CurlevelDetail = levelObj.GetComponent<LevelDetails>();
        UIManager.instance.SetGame();
        ShowMsg();
    }

    public int getLevel
    {
        get { return PlayerPrefs.GetInt("currentLevel", 1); }
        set
        {
            if (value > levelsList.Count) return;
            PlayerPrefs.SetInt("currentLevel", value);
            PlayerPrefs.SetInt(PlayerPrefs.GetString("PlayerUserId"), value);
            // LeaderboardManager.manager.SendLeaderboard(PlayerPrefs.GetInt("currentLevel"));
        }
    }

    void ShowMsg()
    {
        msgBg.DOFade(0.7f, 0.5f);
        msgPopup.DOScale(1f, 0.5f).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                msgBg.DOFade(0f, 0.5f).SetDelay(1.5f);
                msgPopup.DOScale(0f, 0.5f).SetEase(Ease.InBack).SetDelay(1.5f).OnComplete(() =>
                {
                    CurlevelDetail.StartTimer();
                    UIManager.instance.PanelSpotDiffrence.SetActive(false);
                });
            });
    }

    public void OpenCoinShop()
    {
        UIManager.instance.OnClick_Shop("Coin");
        Time.timeScale = 0f;
    }

    public void OpenLifesShop()
    {
        UIManager.instance.OnClick_Shop("Lives");
        Time.timeScale = 0f;
    }
}
