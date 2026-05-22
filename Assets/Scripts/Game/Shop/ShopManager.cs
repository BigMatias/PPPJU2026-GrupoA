using System;
using System.Collections.Generic;
using UnityEngine;

/* ----- FACU -----
 * El ShopManager aparece entre rondas de truco
 * En el se pueden comprar gauchos (faltan agregar las cartas)
 * Tiene un metodo extra sin usar que es para rerollear, que estaria bueno agregarlo post-parcial 1
*/

public class ShopManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private UIShop _uiShop;
    [Header("Gaucho Settings")]
    [SerializeField] private int _gauchoSlots = 3;
    [SerializeField] private int _rerollCost = 5;
    [SerializeField] private List<GauchoDataSO> _gauchosPool = new();

    [Header("Card Settings")]
    [SerializeField] private int _cardSlots = 2;
    [SerializeField] private int _baseCardCost = 5;
    [SerializeField] private List<CardDataSO> _cardsPool = new();
    [SerializeField] private GameObject _cardShopItemPrefab;

    private List<ShopGauchoSlot> _showingGauchos = new();
    private List<CardShopSlot> _showingCards = new();
    public void OpenShop(Action onContinue)
    {
        SelectGauchosToBuy();
        SelectCardsToBuy();
        _uiShop.Open(onContinue);
    }
    public void CloseShop()
    {
        foreach (ShopGauchoSlot slot in _showingGauchos)
            if (slot.go != null) Destroy(slot.go);
        _showingGauchos.Clear();

        foreach (CardShopSlot slot in _showingCards)
            if (slot.go != null) Destroy(slot.go);
        _showingCards.Clear();
    }

    // ── Gauchos ───────────────────

    private void SelectGauchosToBuy()
    {
        List<GauchoDataSO> available = new(_gauchosPool);
        for (int i = 0; i < _gauchoSlots; i++)
        {
            if (available.Count <= 0) break;
            int rand = UnityEngine.Random.Range(0, available.Count);
            GauchoDataSO selected = available[rand];
            ShowGaucho(selected, i);
            available.RemoveAt(rand);
        }
    }
    private void ShowGaucho(GauchoDataSO gaucho, int slotIndex)
    {
        GameObject go = Instantiate(gaucho.prefabShop);
        ShopGauchoSlot slot = new()
        {
            data = gaucho,
            go = go,
            slotIndex = slotIndex
        };
        _showingGauchos.Add(slot);
        go.GetComponent<ShopItem>().Setup(this, slot);
        go.GetComponent<UiShopItem>().Setup(gaucho, go.GetComponent<ShopItem>());
    }
    public void BuyGaucho(ShopGauchoSlot slot)
    {
        if (RunManager.Instance.MoneySystem.CurrentMoney < slot.data.cost)
        {
            return;
        }
        RunManager.Instance.MoneySystem.SubstractMoney(slot.data.cost);
        RunManager.Instance.Gauchos.AddGauchoToRun(slot.data);
        if (slot.go != null) Destroy(slot.go);
        _showingGauchos.Remove(slot);
        _uiShop.RefreshMoney();
    }
    public void Reroll()
    {
        if (RunManager.Instance.MoneySystem.CurrentMoney < _rerollCost) return;
        RunManager.Instance.MoneySystem.SubstractMoney(_rerollCost);
        CloseShop();
        SelectGauchosToBuy();
        SelectCardsToBuy();
        _uiShop.RefreshMoney();
    }
    // ── Cartas ────────────────────────────────────────────
    private void SelectCardsToBuy()
    {
        List<CardDataSO> available = new(_cardsPool);
        for (int i = 0; i < _cardSlots; i++)
        {
            if (available.Count <= 0) break;
            int rand = UnityEngine.Random.Range(0, available.Count);
            CardDataSO selected = available[rand];
            ShowCard(selected, i);
            available.RemoveAt(rand);
        }
    }
    private void ShowCard(CardDataSO cardData, int slotIndex)
    {
        GameObject go = Instantiate(_cardShopItemPrefab);

        CardShopSlot slot = new()
        {
            data = cardData,
            go = go,
            slotIndex = slotIndex,
            cost = _baseCardCost
        };
        _showingCards.Add(slot);

        ShopCardItem shopCardItem = go.GetComponent<ShopCardItem>();
        shopCardItem.Setup(this, slot);

        UIShopCardItem uiItem = go.GetComponent<UIShopCardItem>();
        uiItem.Setup(cardData, _baseCardCost, shopCardItem);
    }
    public void BuyCard(CardShopSlot slot)
    {
        if (RunManager.Instance.MoneySystem.CurrentMoney < slot.cost)
        {
            Debug.Log("No hay plata para comprar la carta");
            return;
        }

        // ¿Ya hay una carta pendiente? Por ahora solo permitimos una a la vez
        if (RunManager.Instance.CardSwapManager.HasPendingCard)
        {
            Debug.Log("Ya tenés una carta pendiente de intercambio");
            return;
        }

        RunManager.Instance.MoneySystem.SubstractMoney(slot.cost);
        RunManager.Instance.CardSwapManager.SetPendingCard(slot.data);

        if (slot.go != null) Destroy(slot.go);
        _showingCards.Remove(slot);

        _uiShop.RefreshMoney();
        Debug.Log($"Compraste la carta: {slot.data.name}");
    }
}