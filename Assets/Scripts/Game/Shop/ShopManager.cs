using System.Collections.Generic;
using UnityEngine;

/* ----- FACU -----
 * El ShopManager aparece entre rondas de truco
 * En el se pueden comprar gauchos (faltan agregar las cartas)
 * Tiene un metodo extra sin usar que es para rerollear, que estaria bueno agregarlo post-parcial 1
*/

public class ShopManager : MonoBehaviour
{
    [Header("Shop Settings")]
    [SerializeField] private int _gauchoSlots = 3;
    [SerializeField] private int _rerollCost = 5;

    [Header("Gaucho Pool")]
    [SerializeField] private List<GauchoDataSO> _gauchosPool = new();

    private List<ShopGauchoSlot> _showingGauchos = new();

    public void OpenShop() => SelectGauchosToBuy();

    public void CloseShop()
    {
        foreach (ShopGauchoSlot slot in _showingGauchos)
            if (slot.go != null)
                Destroy(slot.go);

        _showingGauchos.Clear();
    }

    private void SelectGauchosToBuy()
    {
        List<GauchoDataSO> available = new(_gauchosPool);
        for (int i = 0; i < _gauchoSlots; i++)
        {
            if (available.Count <= 0)
                break;

            int rand = Random.Range(0, available.Count);
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
        ShopItem shopItem = go.GetComponent<ShopItem>();
        shopItem.Setup(this, slot);

        UiShopItem visual = go.GetComponent<UiShopItem>();
        visual.Setup(gaucho, shopItem);
    }

    public void BuyGaucho(ShopGauchoSlot slot)
    {
        if (RunManager.Instance.MoneySystem.CurrentMoney < slot.data.cost)
        {
            Debug.Log("No hay plata");
            return;
        }

        RunManager.Instance.MoneySystem.SubstractMoney(slot.data.cost);
        RunManager.Instance.Gauchos.AddGauchoToRun(slot.data);

        if (slot.go != null)
            Destroy(slot.go);

        _showingGauchos.Remove(slot);
        Debug.Log($"Compraste " + $"{slot.data.name}");
    }

    public void Reroll()
    {
        if (RunManager.Instance.MoneySystem.CurrentMoney < _rerollCost)
            return;

        RunManager.Instance.MoneySystem.SubstractMoney(_rerollCost);
        CloseShop();
        OpenShop();
    }
}