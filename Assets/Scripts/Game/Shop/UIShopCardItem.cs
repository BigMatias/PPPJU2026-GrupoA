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
        _btnBuy = GetComponentInChildren<Button>();
    }
    private void Start()
    {
        if (_btnBuy == null)
            _btnBuy = GetComponent<Button>();
        _btnBuy.onClick.AddListener(OnBuyClicked);
    }
    private void OnDestroy()
    {
        if (_btnBuy != null)
            _btnBuy.onClick.RemoveAllListeners();
    }
    public void Setup(CardDataSO data, int cost, ShopCardItem shopCardItem)
    {
        if (_btnBuy == null)
            _btnBuy = GetComponentInChildren<Button>();

        _shopCardItem = shopCardItem;

        if (_cardImage != null) _cardImage.sprite = data.artwork;
        if (_textName != null)
            _textName.text = data.name.Replace("CardDataSO", "");
        if (_textCost != null) _textCost.text = "$" + cost;
    }
    private void OnBuyClicked() => _shopCardItem.Buy();
}