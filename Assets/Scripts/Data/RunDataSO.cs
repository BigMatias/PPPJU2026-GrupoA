using UnityEngine;

[CreateAssetMenu(fileName = "RunData", menuName = "GameSettings/RunData")]

public class RunDataSO : ScriptableObject
{
    public int totalHands;
    public int handsPlayed;
    public int money;
    public int points;
    public int baseMult;
}