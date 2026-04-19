using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopTimeInfo : MonoBehaviour
{
    [SerializeField] int seconds, coins;
    [SerializeField] TextMeshProUGUI txtseconds, txtcoins;

    private void Start()
    {
        txtseconds.text = seconds + " sec";
        txtcoins.text = "" + coins;
    }

    public void OnClickBuy()
    {
        if (coins <= GameManager.instance.getsetCoins)
        {
            GameManager.instance.getsetCoins -= coins;
            GameManager.instance.ShowToastMsg("Got " + seconds + " seconds!");
            LevelManager.instance.CurlevelDetail.totalTime = seconds;
            LevelManager.instance.CurlevelDetail.currentTime = seconds;
            LevelManager.instance.CurlevelDetail.continueGame(true);
        }
        else GameManager.instance.ShowToastMsg("Not enought coins!");
    }
}
