using UnityEngine;

public class ShopManager : MonoBehaviour
{
   /* [SerializeField] private RunDataSO _runData;

    public void ShowJokersToBuy()
    {
        int rand = (int)(Random.value * GauchosManager.Instance.GetJokersList().Count);

        GauchosManager.Instance.GetJokersList()[rand].ToggleJokerOnShop(true);
    }

    public void ShowCards()
    {
        // aca necesitamos la lista de cartas para el random :p
    }

    public bool BuyJoker(Joker joker)
    {
        if (_runData.money < joker.jokerData.cost)
            return false;

        _runData.money -= joker.jokerData.cost;

        GauchosManager.Instance.JokerBoughtAndAddedToHand(joker);
    
        return true;
    }

    public void CloseShop()
    {
        foreach (Joker item in GauchosManager.Instance.GetJokersList())
            if (item.isVisibleInShop)
                item.ToggleJokerOnShop(false);
    } */
}