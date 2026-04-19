using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopLifeInfo : MonoBehaviour
{
    [SerializeField] int lifes, coins;
    [SerializeField] TextMeshProUGUI txtlifes, txtcoins;

    private void Start()
    {
        txtlifes.text = lifes.ToString();
        txtcoins.text = "" + coins;
    }

    public void OnClickBuy()
    {
        if (coins <= GameManager.instance.getsetCoins)
        {
            GameManager.instance.getsetCoins -= coins;
            GameManager.instance.getsetLife += lifes;
            GameManager.instance.ShowToastMsg("Got " + lifes + " more lifes!");
            if (GameManager.instance.isGameFail) LevelManager.instance.CurlevelDetail.continueGame(false);
        }
        else GameManager.instance.ShowToastMsg("Not enought coins!");
    }
}
