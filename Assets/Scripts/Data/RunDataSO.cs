using UnityEngine;

[CreateAssetMenu(fileName = "RunData", menuName = "GameSettings/RunData")]
public class RunDataSO : ScriptableObject
{
    [Header("Run")]
    public int totalHands;
    public int handsPlayed;
    public int pointsNeededToWin;
    public int points;

    [Header("Score Base")]
    public float basePoints = 5f;
    public float baseMult = 1f;

    [Header("Hand")]
    public int handsPerRound = 3;
    public float chanceToBeDealer = 0.5f;

    [Header("Card Points")]
    public float cardPointMultiplier = 1f;

    [Header("Envido")]
    public float envidoPoints = 10f;
    public float envidoEnvidoPoints = 20f;
    public float realEnvidoPoints = 30f;
    public float faltaEnvidoPoints = 50f;

    [Header("Truco Multipliers")]
    public float trucoMult = 1.5f;
    public float retrucoMult = 2f;
    public float valeCuatroMult = 3f;
}