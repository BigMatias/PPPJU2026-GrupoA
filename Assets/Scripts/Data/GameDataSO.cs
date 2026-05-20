using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "GameSettings/GameData")]
public class GameDataSO : ScriptableObject
{
    public int totalPoints;
    public int pointsNeededToWinRound;
}