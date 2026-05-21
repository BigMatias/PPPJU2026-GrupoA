using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiShopItem : MonoBehaviour
{
    [SerializeField] private Button _btnBuy;
    [SerializeField] private TMP_Text _textCost;
    [SerializeField] private TMP_Text _textName;
    private ShopItem _shopItem;

    private void Start()
    {
        _btnBuy.onClick.AddListener(ButtonClicked);
    }

    private void OnDestroy()
    {
        _btnBuy.onClick.RemoveAllListeners();
    }

    public void Setup(GauchoDataSO data, ShopItem shopItem)
    {
        _shopItem = shopItem;
        _btnBuy.image.sprite = data.sprite;
        _textCost.text = "$" + data.cost.ToString();
        _textName.text = data.name;
    }

    private void ButtonClicked() => _shopItem.Buy();
}