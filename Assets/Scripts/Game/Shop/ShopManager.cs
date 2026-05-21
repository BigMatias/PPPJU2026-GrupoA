using System.Collections.Generic;
using UnityEngine;

/* ----- FACU -----
 * El ShopManager aparece entre rondas de truco
 * Tiene sus listas que
*/

public class ShopManager : MonoBehaviour
{
    [Header("Run Data")]
    [SerializeField] private RunDataSO _runData;

    [Header("Shop Settings")]
    [SerializeField] private int _gauchoSlots = 3;
    [SerializeField] private int _rerollCost = 5;

    [Header("Positions")]
    [SerializeField] private List<Transform> _gauchoPositions = new();

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
        go.transform.position = _gauchoPositions[slotIndex].position;

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
        if (_runData.money < slot.data.cost)
        {
            Debug.Log("No hay plata");
            return;
        }

        _runData.money -= slot.data.cost;
        RunManager.Instance.Gauchos.AddGauchoToRun(slot.data);

        if (slot.go != null)
            Destroy(slot.go);

        _showingGauchos.Remove(slot);
        Debug.Log($"Compraste " + $"{slot.data.name}");
    }

    public void Reroll()
    {
        if (_runData.money < _rerollCost)
            return;

        _runData.money -= _rerollCost;
        CloseShop();
        OpenShop();
    }
}