using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class LevelDetails : MonoBehaviour, IPointerDownHandler
{

    public static UnityAction winGameCheck;
    public List<Sprite> allThisLevelSprite = new List<Sprite>();
    public int totalWrongSelect;
    //public int currentWrongSelect;
    public int totalrights, rightscount;
    public int totalTime;
    public int currentTime;
    public bool isTimebase;
    public GameObject Prefab_Cancel;
    public LevelType levelType;

    public enum LevelType
    {
        Timebase,
        wrongSelection
    }

    private void Awake()
    {
        winGameCheck += CheckWinGame;
    }

    private void OnDestroy()
    {
        winGameCheck -= CheckWinGame;
    }

    void Start()
    {
        //Reset Value
        GameManager.instance.TxtCorrect.text = rightscount + "/" + totalrights;
        GameManager.instance.TxtTimer.text = totalTime.ToString();
        GameManager.instance.isWinGame = false;
        GameManager.instance.isGameFail = false;
        GameManager.instance.hintbtn.SetActive(true);
        LevelManager.instance.isHintClickable = true;

        if (!PlayerPrefs.HasKey("BonusHintOf" + PlayerPrefs.GetInt("currentLevel")))
        {
            PlayerPrefs.SetInt("BonusHintOf" + PlayerPrefs.GetInt("currentLevel"), 1);
        }

        LevelManager.instance.txthint.text = "Hint(" + (GameManager.instance.getsetHint + PlayerPrefs.GetInt("BonusHintOf" + PlayerPrefs.GetInt("currentLevel"))) + ")";

        LevelManager.instance.Index.Clear();
        for (int i = 0; i < 5; i++) LevelManager.instance.Index.Add(i);

        for (int i = 0; i < transform.childCount; i++) transform.GetChild(i).gameObject.SetActive(false);

        if (!PlayerPrefs.HasKey("Level_" + PlayerPrefs.GetInt("currentLevel") + "_of_"))
            PlayerPrefs.SetInt("Level_" + PlayerPrefs.GetInt("currentLevel") + "_of_", 1);

        if (PlayerPrefs.GetInt("Level_" + PlayerPrefs.GetInt("currentLevel") + "_of_") > allThisLevelSprite.Count)
        {
            PlayerPrefs.SetInt("Level_" + PlayerPrefs.GetInt("currentLevel") + "_of_", 1);
        }

        int nextLevel = PlayerPrefs.GetInt("Level_" + PlayerPrefs.GetInt("currentLevel") + "_of_") - 1;

        GetComponent<Image>().sprite = allThisLevelSprite[nextLevel];
        LevelManager.instance.Mistackes = transform.GetChild(nextLevel).gameObject;
        transform.GetChild(nextLevel).gameObject.SetActive(true);

        if (levelType == LevelType.Timebase)
        {
            currentTime = totalTime;
            isTimebase = true;
        }
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);
        GameManager.instance.TxtTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void continueGame(bool via_Time)
    {
        if (via_Time) GameManager.instance.TxtTimer.text = totalTime.ToString();
        else
        {
            if (levelType == LevelType.Timebase)
            {
                if (currentTime < 10) currentTime = 20;
                if (totalTime < 20) totalTime = 20;
            }
        }
        GameManager.instance.isWinGame = false;
        GameManager.instance.isGameFail = false;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);
        GameManager.instance.TxtTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        UIManager.instance.PanelGameOver.SetActive(false);
        UIManager.instance.OnClick_Shop_Close();

        StartTimer();
    }

    public void StartTimer()
    {
        StartCoroutine(StartGameTimer());
    }

    IEnumerator StartGameTimer()
    {
        if (isTimebase)
        {
            Debug.Log("Istime");
            while (currentTime >= 0)
            {
                float minutes = Mathf.FloorToInt(currentTime / 60);
                float seconds = Mathf.FloorToInt(currentTime % 60);
                GameManager.instance.TxtTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
                if (currentTime < 10) SoundHapticManager.Instance.playClip(SoundHapticManager.Instance.timer, 1);
                yield return new WaitForSeconds(1f);
                currentTime -= 1;
                if (GameManager.instance.isWinGame || GameManager.instance.isGameFail) break;
            }
            if (!GameManager.instance.isWinGame && !GameManager.instance.isGameFail)
            {
                GameManager.instance.isGameFail = true;
                UIManager.instance.SetGameFail("Time");
                UIManager.instance.txtgamelosereason.text = "You have run out of time! Get extra time or abort mission";
            }
        }
        else
        {

            while (true)
            {
                float minutes = Mathf.FloorToInt(currentTime / 60);
                float seconds = Mathf.FloorToInt(currentTime % 60);
                GameManager.instance.TxtTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
                yield return new WaitForSeconds(1f);
                currentTime++;
                if (GameManager.instance.isWinGame || GameManager.instance.isGameFail) break;
            }
        }
    }

    void CheckWinGame()
    {
        if (rightscount >= 5)
        {

            GameManager.instance.isWinGame = true;
            UIManager.instance.SetGameWin();
        }
    }

    void WrongSelection()
    {
        LevelManager.instance.heartParticle.Play();
        //currentWrongSelect++;
        GameManager.instance.getsetLife--;
        GameManager.instance.TxtCorrect.text = rightscount + "/" + totalrights;
        //if (totalWrongSelect <= currentWrongSelect)
        if (GameManager.instance.getsetLife <= 0)
        {
            GameManager.instance.isGameFail = true;
            UIManager.instance.SetGameFail("Lives");
            UIManager.instance.txtgamelosereason.text = "You have made too many mistakes! Get Extra life or abort mission";
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (EventSystem.current.currentSelectedGameObject == null && Input.GetMouseButtonDown(0) && !UIManager.instance.IsAnyPopUpOpen() && !GameManager.instance.isGameFail && !GameManager.instance.isWinGame)
        {
            if (FindDiff.OverCollider == false)
            {
                SoundHapticManager.Instance.playClip(SoundHapticManager.Instance.wrongSelect, 1);
                SoundHapticManager.Instance.Haptic();
                GameObject obj = Instantiate(Prefab_Cancel, LevelManager.instance.UiCanvas);

                Vector2 anchorposition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(LevelManager.instance.UiCanvas, Input.mousePosition, Camera.main, out anchorposition);
                obj.transform.GetComponent<RectTransform>().anchoredPosition = anchorposition;
                obj.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBounce);
                obj.GetComponent<Image>().DOFade(0f, 0.5f).SetDelay(0.5f).OnComplete(() =>
                {
                    Destroy(obj);
                });
                WrongSelection();
            }
        }
    }
}
