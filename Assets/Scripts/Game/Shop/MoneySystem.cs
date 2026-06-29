using System;
using UnityEngine;

public class MoneySystem : MonoBehaviour
{
    public event Action OnUpdateMoney;

    [SerializeField] private MoneyDataSO _data;

    private int _currentMoney = 0;
    public int CurrentMoney => _currentMoney;

    private void Start()
    {
        _currentMoney = 0;
    }

    public void AddMoneyForWinningRound(int extraHands)
    {
        int calculation = _data.moneyForRound + (_data.moneyForRound * extraHands);
        int interest = calculation / _data.moneyNeededForInterest;

        _currentMoney += calculation + interest;

        OnUpdateMoney?.Invoke();
    }

    public void AddMoney(int money)
    {
        _currentMoney += money;
        OnUpdateMoney?.Invoke();
    }

    public void SubstractMoney(int moneyToSubstract)
    {
        _currentMoney -= moneyToSubstract;
        OnUpdateMoney?.Invoke();
    }
}