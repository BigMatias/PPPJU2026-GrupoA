using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameDefinition", menuName = "GameData/GameDefinition")]
public class GameDefinitionSO : ScriptableObject
{
    public List<CardDataSO> cards;

    public float battleLevel;
    public bool isWon;
}
