using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* ----- FACU -----
 * Este script es la parte de UI de cada carta en la tienda
 * Tiene el botón, el texto del nombre y el texto del coste
*/

public class UiShopItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _textCost;
    [SerializeField] private TMP_Text _textName;
    private Button _btnBuy;
    private ShopItem _shopItem;

    private void Awake()
    {
        _btnBuy = GetComponent<Button>();
    }

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