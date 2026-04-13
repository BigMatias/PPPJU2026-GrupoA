using UnityEngine;

[CreateAssetMenu(fileName = "RunData", menuName = "GameSettings/RunData")]

public class RunDataSO : ScriptableObject
{
    public int totalHands;
    public int handsPlayed;
    public int money;
}