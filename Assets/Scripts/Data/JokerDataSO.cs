using UnityEngine;

[CreateAssetMenu(fileName = "JokerData", menuName = "CardsData/JokerData")]

public class JokerDataSO : ScriptableObject
{
    public GameObject prefab;

    public string jokerName;

    [TextArea]
    public string description;

    public int cost;

    public Rarity rarity;

    [Header("Points to add")]
    public float bonusPoints;

    [Header("Mult to add")]
    public float bonusMultAdd;

    [Header("Mult to multiply")]
    public float bonusMultX;

    [Header("Additive mult to add")]
    public float bonusMultAddAdditive;
    public float bonusMultAddAdditiveTotal;

    [Header("Additive mult to multiply")]
    public float bonusMultXAdditive;
    public float bonusMultXAdditiveTotal;
}