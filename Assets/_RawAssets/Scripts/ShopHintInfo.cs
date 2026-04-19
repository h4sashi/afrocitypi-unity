using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopHintInfo : MonoBehaviour
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
        // IAPManager.manager.Buy_hint(productkey);
    }

    public void OnSuccessHint()
    {
        GameManager.instance.ShowToastMsg(noofValue + " Hints Added");
        GameManager.instance.getsetHint += noofValue;
    }
}
