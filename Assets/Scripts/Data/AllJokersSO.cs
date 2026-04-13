using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllJokers", menuName = "Jokers/AllJokers")]

public class AllJokersSO : ScriptableObject
{
    public List<Joker> _jokersList = new List<Joker>();
}