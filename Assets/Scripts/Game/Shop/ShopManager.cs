using UnityEngine;

public class ShopManager : MonoBehaviour
{
   [SerializeField] private RunDataSO _runData;

    public void ShowJokersToBuy()
    {

    }

    public void ShowCards()
    {
        // aca necesitamos la lista de cartas para el random :p
    }

    public void BuyJoker(GauchoInstance gaucho)
    {
        if (_runData.money >= gaucho.data.cost)
        {
            _runData.money -= gaucho.data.cost;
            // algo en UI que reste la plata gastada
        }

        GauchoInstance newGaucho = RunManager.Instance.Gauchos.GetInactiveGaucho(gaucho); // saco el gaucho
        RunManager.Instance.Gauchos.SetNewGaucho(newGaucho);
    }

    public void CloseShop()
    {

    }
}