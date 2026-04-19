using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCoinInfo : MonoBehaviour
{
    [SerializeField] string productkey;
    [SerializeField] int noofValue;
    [SerializeField] Text txtprice;
    [SerializeField] TextMeshProUGUI txtnoofvalue;
    [SerializeField] Button BtnBuy;

    private void Start()
    {
        txtnoofvalue.text = noofValue.ToString();
    }

    public void SetPrice(string price)
    {
        txtprice.text = price;
        BtnBuy.interactable = true;
    }

    public void OnClickBuy()
    {
        if (productkey == "" || productkey == null)
        {
            // YodoManager.manager.ShowRewarded(3);
        }
        // else
            // IAPManager.manager.Buy_coin(productkey);
    }

    public void OnSuccessCoin()
    {
        GameManager.instance.ShowToastMsg(noofValue + " Coins Added");
        GameManager.instance.getsetCoins += noofValue;
    }
}
