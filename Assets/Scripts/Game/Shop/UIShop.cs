using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIShop : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject _shopPanel;
    [SerializeField] private TMP_Text _moneyText;
    [SerializeField] private Button _continueButton;

    private Action _onContinue;

    private void Start()
    {
        _continueButton.onClick.AddListener(OnContinueClicked);
        _shopPanel.SetActive(false);
    }
    private void OnDestroy()
    {
        _continueButton.onClick.RemoveAllListeners();
    }
    public void Open(Action onContinue)
    {
        _onContinue = onContinue;
        RefreshMoney();
        _shopPanel.SetActive(true);
    }
    private void OnContinueClicked()
    {
        _shopPanel.SetActive(false);

        RunManager.Instance.ShopManager.CloseShop();

        _onContinue?.Invoke();
        _onContinue = null;
    }
    public void RefreshMoney()
    {
        _moneyText.text = "$" + RunManager.Instance.MoneySystem.CurrentMoney;
    }
}