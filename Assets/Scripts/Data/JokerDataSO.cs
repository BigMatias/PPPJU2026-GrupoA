using UnityEngine;

[CreateAssetMenu(fileName = "JokerData", menuName = "CardsData/JokerData")]

public partial class JokerDataSO : ScriptableObject
{
    public Sprite sprite;
    public Rarity rarity;
    public int pointsEffect;
    public int multEffect;
}