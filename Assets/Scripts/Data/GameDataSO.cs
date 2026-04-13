using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "GameSettings/GameData")]

public class GameDataSO : ScriptableObject
{
    public int jokersDiscovered;
    public int roundsWon;
    public int trucoPoints;
    public int envidoPoints;
    public int totalPoints;
}