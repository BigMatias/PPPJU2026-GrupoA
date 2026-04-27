using UnityEngine;
public class ShopManager :MonoBehaviour
{
    [SerializeField] private AllJokersSO _allJokers;
    [SerializeField] private RunDataSO _runData;

    public void BuyJoker(JokerDataSO joker)
    {
        if (_runData.money < joker.cost)
            return;

        _runData.money -= joker.cost;

        JokerManager.instance.AddJoker(joker);
    }
}