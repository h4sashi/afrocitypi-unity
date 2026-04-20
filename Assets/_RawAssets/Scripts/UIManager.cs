using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [Header("Panels")]
    public GameObject[] popUpAry;
    public GameObject PanelSplashScreen, PanelGameWin, PanelGameOver, PanelGamePlay, PanelHomeScreen, PanelSpotDiffrence, PanelLevelSelection, PanelSettings, PanelProfile, PanelShop, PanelLeaderBoard;
    public TextMeshProUGUI txtgamelosereason, txtSpotDifference;


    [Header("Profile")]
    public TextMeshProUGUI txtUsername;
    public TextMeshProUGUI txtUserlevel, txtUsercoin;
    public Image userprofile1, userprofile2;

    [Header("Values update")]
    public List<TextMeshProUGUI> hintTextList;
    public List<TextMeshProUGUI> coinTextList;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        // Refresh profile when scene loads (especially for Game scene)
        RefreshUserProfile();
    }

    private void Start()
    {
        UpdateCoins();
        UpdateHint();
        SetSplashScreen();
        SetUserProfile();
    }

    public bool IsAnyPopUpOpen()
    {
        if (PanelGamePlay.activeSelf && !PanelSpotDiffrence.activeSelf && !PanelGameOver.activeSelf) return false;
        return true;
    }

    public void PopUpManager(GameObject obj)
    {
        if (PanelSpotDiffrence == obj || PanelGameOver == obj || PanelGameWin == obj)
        {
            obj.SetActive(true);
            return;
        }
        for (int i = 0; i < popUpAry.Length; i++)
        {
            popUpAry[i].SetActive(false);
        }
        obj.SetActive(true);
    }

    public void SetSplashScreen()
    {
        PopUpManager(PanelSplashScreen);
        Invoke(nameof(SetHomeScreen), 1f);
    }

    public void SetHomeScreen()
    {
        if (LevelManager.instance.CurlevelDetail != null)
        {
            Destroy(LevelManager.instance.CurlevelDetail.gameObject);
            LevelManager.instance.CurlevelDetail = null;
        }
        PopUpManager(PanelHomeScreen);
    }

    public void SetLevelSelections()
    {
        PopUpManager(PanelLevelSelection);
    }

    public void SetGame()
    {
        PopUpManager(PanelGamePlay);
        if (LevelManager.instance.CurlevelDetail.levelType == LevelDetails.LevelType.Timebase)
            txtSpotDifference.text = "Spot 5 differences in " + LevelManager.instance.CurlevelDetail.totalTime + " secs to complete this level";
        else if (LevelManager.instance.CurlevelDetail.levelType == LevelDetails.LevelType.wrongSelection)
            txtSpotDifference.text = "Spot 5 differences to complete this level";
        PopUpManager(PanelSpotDiffrence);
    }

    public void SetGameWin()
    {
        SoundHapticManager.Instance.playClip(SoundHapticManager.Instance.win, 1);
        SoundHapticManager.Instance.Haptic();
        LevelManager.instance.getLevel++;
        StartCoroutine(Wait("GameWin", .5f));
    }

    public void SetGameFail(string str)
    {
        str_shopOnFail = str;
        SoundHapticManager.Instance.playClip(SoundHapticManager.Instance.fail, 1);
        SoundHapticManager.Instance.Haptic();
        StartCoroutine(Wait("GameFail", 0.5f));
    }

    IEnumerator Wait(string EventName, float time)
    {
        switch (EventName)
        {
            case "GameWin":
                {
                    yield return new WaitForSeconds(time);
                    PopUpManager(PanelGameWin);
                    break;
                }
            case "GameFail":
                {
                    yield return new WaitForSeconds(time);
                    PopUpManager(PanelGameOver);
                    break;
                }
        }
        PlayerPrefs.SetInt("Level_" + PlayerPrefs.GetInt("currentLevel") + "_of_", PlayerPrefs.GetInt("Level_" + PlayerPrefs.GetInt("currentLevel") + "_of_") + 1);

    }

    public void OnClick_LeaderBoard()
    {
        //PanelLeaderBoard.SetActive(true);
        // LeaderboardManager.manager.ShowLeaderboard();
    }

    public void OnClick_LeaderBoard_Close()
    {
        PanelLeaderBoard.SetActive(false);
    }

    string str_shopOnFail;
    public void OpenShop_OnClick_LevelFailed()
    {
        OnClick_Shop(str_shopOnFail);
    }

    public void OnClick_Shop(string typeOfShop)
    {
        PanelShop.SetActive(true);
        switch (typeOfShop)
        {
            case "Coin":
                {
                    Shop.manager.CoinShopMenu();
                    break;
                }
            case "Hint":
                {
                    Shop.manager.HintShopMenu();
                    break;
                }
            case "Lives":
                {
                    Shop.manager.LivesShopMenu();
                    break;
                }
            case "Time":
                {
                    Shop.manager.TimeShopMenu();
                    break;
                }
        }
    }

    public void OnClick_Shop_Close()
    {
        PanelShop.SetActive(false);
        Shop.manager.liveBtn.gameObject.SetActive(false);
        Shop.manager.timeBtn.gameObject.SetActive(false);

        Shop.manager.liveShop.gameObject.SetActive(false);
        Shop.manager.timeShop.gameObject.SetActive(false);

        Time.timeScale = 1f;
    }

    public void OnClick_Profile()
    {
        txtUserlevel.text = "" + LevelManager.instance.getLevel;
        PanelProfile.SetActive(true);
    }

    public void OnClick_Profile_Close()
    {
        PanelProfile.SetActive(false);
    }

    public void OnClick_Setting()
    {
        PanelSettings.SetActive(true);
    }

    public void OnClick_Setting_Close()
    {
        PanelSettings.SetActive(false);
    }

    public void OnClickLogout()
    {
        // LoginManager.manager.LogOut();
        /*Destroy(LoginManager.manager.gameObject);*/
        SceneManager.LoadScene(1);
    }


    void SetUserProfile()
    {
        // Clear existing sprites
        userprofile1.sprite = null;
        userprofile2.sprite = null;

        // Try to get profile from PlayFabManager (works across scenes)
        if (PlayFabManager.Instance != null)
        {
            var userProfile = PlayFabManager.Instance.GetCurrentUserProfile();
            if (userProfile != null)
            {
                // Set username if available
                if (txtUsername != null)
                {
                    txtUsername.text = userProfile.given_name ?? userProfile.name ?? "Player";
                }

                // Download and set profile picture
                PlayFabManager.Instance.DownloadProfilePicture((texture) =>
                {
                    if (texture != null)
                    {
                        Sprite profileSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                        userprofile1.sprite = profileSprite;
                        userprofile2.sprite = profileSprite;
                        Debug.Log("Profile images set successfully");
                    }
                    else
                    {
                        Debug.LogWarning("Failed to download profile picture");
                    }
                });
            }
            else
            {
                Debug.Log("No user profile available from PlayFabManager");
            }
        }
        else
        {
            Debug.LogWarning("PlayFabManager instance not found");
        }
    }

    public void RefreshUserProfile()
    {
        SetUserProfile();
    }

    public void UpdateHint()
    {
        for (int i = 0; i < hintTextList.Count; i++)
        {
            hintTextList[i].text = GameManager.instance.getsetHint.ToString();
        }
        LevelManager.instance.txthint.text = "Hint(" + (GameManager.instance.getsetHint + PlayerPrefs.GetInt("BonusHintOf" + PlayerPrefs.GetInt("currentLevel"))) + ")";
    }

    public void UpdateCoins()
    {
        for (int i = 0; i < coinTextList.Count; i++)
        {
            coinTextList[i].text = GameManager.instance.getsetCoins.ToString();
        }
    }

    public void OnGameOverAdsShow()
    {
        //1 for time
        //2 for lives
        if (LevelManager.instance.CurlevelDetail.levelType == LevelDetails.LevelType.Timebase)
        {
            // YodoManager.manager.ShowRewarded(1);
        }
        else if (LevelManager.instance.CurlevelDetail.levelType == LevelDetails.LevelType.wrongSelection)
        {
            // YodoManager.manager.ShowRewarded(2);
        }

    }
}
