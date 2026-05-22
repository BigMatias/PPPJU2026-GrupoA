using UnityEngine;

public class MoneySystem : MonoBehaviour
{
    [SerializeField] private MoneyDataSO _data;

    private int _currentMoney = 0;
    public int CurrentMoney => _currentMoney;

    private void Start()
    {
        _currentMoney = 0;
    }

    public int AddMoneyForWinningRound(int extraHands)
    {
        int calculation = _data.moneyForRound + (_data.moneyForRound * extraHands);
        int interest = calculation / _data.moneyNeededForInterest;

        _currentMoney += calculation + interest;

        return _currentMoney;
    }

    public int SubstractMoney(int moneyToSubstract)
    {
        _currentMoney -= moneyToSubstract;
        return _currentMoney;
    }
}