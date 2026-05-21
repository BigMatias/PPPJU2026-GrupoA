using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiShopItem : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _costText;

    public void Setup(GauchoDataSO data)
    {
        _icon.sprite = data.sprite;
        _nameText.text = data.name;
        _costText.text = "$" + data.cost.ToString();
    }
}