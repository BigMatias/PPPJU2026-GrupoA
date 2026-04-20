using UnityEngine;

[CreateAssetMenu(fileName = "JokerData", menuName = "CardsData/JokerData")]

public partial class JokerDataSO : ScriptableObject
{
    public string jokerName;

    [TextArea]
    public string description;

    public int cost;

    public Rarity rarity;

    public int bonusPoints;
    public int bonusMultAdd;
    public float bonusMultX;
    public float bonusMultXAdditive;
    public float bonusMultXAdditiveTotal;

    public Sprite sprite;
}