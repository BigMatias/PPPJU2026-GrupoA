using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Cards/Card")]
public class CardDataSO : ScriptableObject
{
    public int value;
    public string suit;
    public Sprite artwork;
}