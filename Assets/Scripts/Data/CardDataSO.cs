using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Cards/Card")]
public class CardDataSO : ScriptableObject
{
    public int value;
    public int strength;
    public Suit suit;
    public Sprite artwork;
}