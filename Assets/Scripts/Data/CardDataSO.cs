using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Cards/Card")]
public partial class CardDataSO : ScriptableObject
{
    public int value;
    public Suit suit;
    public Sprite artwork;
}