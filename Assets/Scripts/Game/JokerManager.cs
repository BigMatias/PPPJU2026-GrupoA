using System.Collections.Generic;
using UnityEngine;

public class JokerManager : MonoBehaviour
{
    public static JokerManager instance;

    private List<Joker> _ownedJokers = new List<Joker>();

    private void Awake()
    {
        instance = this;
    }

    public void AddJoker(JokerDataSO data)
    {
        Joker joker = new Joker(data);

        _ownedJokers.Add(joker);
    }

    public void ApplyScoreModifiers(ref int chips, ref float mult)
    {
        foreach (var joker in _ownedJokers)
            joker.ModifyScore(ref chips, ref mult);
    }
}