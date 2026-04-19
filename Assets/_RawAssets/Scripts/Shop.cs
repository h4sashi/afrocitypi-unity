using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    public static Shop manager;
    public GameObject subpanel;
    public Button coinBtn, hintBtn, liveBtn, timeBtn;
    public GameObject coinShop, hintShop, liveShop, timeShop;
    public Color oldSkyFontColor, oldSkyblackBoxColor, newSkyFontColor, newSkyblackBoxColor;
    public List<GameObject> shopList;
    public List<GameObject> BtnList;

    [Space(10)]
    public List<ShopHintInfo> list_hintinfo;
    public List<ShopCoinInfo> list_coinsinfo;
    public List<ShopLifeInfo> list_lifeinfo;
    public List<ShopTimeInfo> list_timeinfo;

    private void Awake()
    {
        manager = this;
    }

    void Start()
    {
        coinBtn.onClick.AddListener(() => CoinShopMenu());
        hintBtn.onClick.AddListener(() => HintShopMenu());
        liveBtn.onClick.AddListener(() => LivesShopMenu());
        timeBtn.onClick.AddListener(() => TimeShopMenu());
    }

    void ChangeShopMenu(GameObject shopObj)
    {
        for (int i = 0; i < shopList.Count; i++)
        {
            shopList[i].SetActive(false);
        }

        for (int i = 0; i < BtnList.Count; i++)
        {
            BtnList[i].GetComponent<Image>().color = oldSkyblackBoxColor;
            BtnList[i].transform.Find("Lable").GetComponent<TextMeshProUGUI>().color = oldSkyFontColor;
        }

        shopObj.SetActive(true);
    }

    #region Direct open Specific Shop
    //This methods assign in editor in specific button
    public void CoinShopMenu()
    {
        ChangeShopMenu(coinShop);
        coinBtn.GetComponent<Image>().color = newSkyblackBoxColor;
        coinBtn.transform.Find("Lable").GetComponent<TextMeshProUGUI>().color = newSkyFontColor;
    }

    public void TimeShopMenu()
    {
        ChangeShopMenu(timeShop);
        timeBtn.GetComponent<Image>().color = newSkyblackBoxColor;
        timeBtn.transform.Find("Lable").GetComponent<TextMeshProUGUI>().color = newSkyFontColor;
        timeBtn.gameObject.SetActive(true);
    }

    public void LivesShopMenu()
    {
        ChangeShopMenu(liveShop);
        liveBtn.GetComponent<Image>().color = newSkyblackBoxColor;
        liveBtn.transform.Find("Lable").GetComponent<TextMeshProUGUI>().color = newSkyFontColor;
        liveBtn.gameObject.SetActive(true);
    }

    public void HintShopMenu()
    {
        ChangeShopMenu(hintShop);
        hintBtn.GetComponent<Image>().color = newSkyblackBoxColor;
        hintBtn.transform.Find("Lable").GetComponent<TextMeshProUGUI>().color = newSkyFontColor;
    }

    #endregion
}