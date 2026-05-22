using UnityEngine;

[CreateAssetMenu(fileName = "MoneyData", menuName = "Money/MoneyData")]

public class MoneyDataSO : ScriptableObject
{
    public int moneyForRound;
    public int moneyForExtraHands;
    public int moneyNeededForInterest;
}