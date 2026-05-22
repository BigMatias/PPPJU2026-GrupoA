using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIShopCardItem : MonoBehaviour
{
    [SerializeField] private Image _cardImage;
    [SerializeField] private TMP_Text _textName;
    [SerializeField] private TMP_Text _textCost;

    private Button _btnBuy;
    private ShopCardItem _shopCardItem;
    private void Awake()
    {
        _btnBuy = GetComponent<Button>();
    }
    private void Start()
    {
        _btnBuy.onClick.AddListener(OnBuyClicked);
    }
    private void OnDestroy()
    {
        _btnBuy.onClick.RemoveAllListeners();
    }
    public void Setup(CardDataSO data, int cost, ShopCardItem shopCardItem)
    {
        _shopCardItem = shopCardItem;
        _cardImage.sprite = data.artwork;
        _textName.text = data.name;
        _textCost.text = "$" + cost;
    }
    private void OnBuyClicked() => _shopCardItem.Buy();
}