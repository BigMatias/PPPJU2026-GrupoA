using System.Collections.Generic;
using UnityEngine;

public class JokerListCreator : MonoBehaviour
{
    [SerializeField] private List<JokerDataSO> _jokersList = new List<JokerDataSO>();
    private Joker _joker;

    private void Awake()
    {
        foreach (JokerDataSO joker in _jokersList)
        {
            Joker newJoker = new Joker(joker);
        }
    }
}