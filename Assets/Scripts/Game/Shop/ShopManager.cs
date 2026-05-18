using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private RunDataSO _runData;
    [SerializeField] private int _gauchoSlots;
    [SerializeField] private int _cardSlots;
    [SerializeField] private List<Vector3> _gauchosPositionInShopList = new();

    private List<GauchoDataSO> _gauchosForShopList = new();
    private List<GauchoDataSO> _showingGauchosInShopList = new();
    private List<GameObject> _gauchosPrefabInShopList = new();

    public void SelectGauchosToBuy()
    {
        for (int i = 0; i < _gauchoSlots; i++)
        {
            int rand = (int)(Random.value * _gauchosForShopList.Count);
            ShowGaucho(_gauchosForShopList[rand], i);
        }
    }

    public void ShowGaucho(GauchoDataSO gaucho, int showingPosition)
    {
        _showingGauchosInShopList.Add(gaucho);
        GameObject go = Instantiate(gaucho.prefab);
        go.transform.position = _gauchosPositionInShopList[showingPosition];
        _gauchosPrefabInShopList.Add(go);
    }

    public void ShowCards()
    {
        // aca necesitamos la lista de cartas para el random :p
    }

    public void BuyJoker(GauchoDataSO gaucho)
    {
        if (_runData.money >= gaucho.cost)
        {
            _runData.money -= gaucho.cost;
            _gauchosForShopList.Remove(gaucho);
            // algo en UI que reste la plata gastada
        }

        RunManager.Instance.Gauchos.AddGauchoToRun(gaucho);
    }

    public void CloseShop()
    {
        foreach (GameObject item in _gauchosPrefabInShopList)
        {
            item.SetActive(false);
            // esto esta MAL y deberia llamar a GauchoInstance.DeActivate(); para desde ahí apagarse
        }
    }
}