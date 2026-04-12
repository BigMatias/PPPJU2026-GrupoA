using UnityEngine;

[CreateAssetMenu(fileName = "JokerData", menuName = "CardsData/JokerData")]

public class JokerDataSO : ScriptableObject
{
    public Sprite sprite;
    public int pointsEffect;
    public int multEffect;
}