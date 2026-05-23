using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private List<CardDataSO> startingDeck;

    private List<CardDataSO> _drawPile = new List<CardDataSO>();

    private void Awake()
    {
        InitializeDeck();
    }

    private void InitializeDeck()
    {
        _drawPile = new List<CardDataSO>(startingDeck);
        Shuffle(_drawPile);
    }

    public CardDataSO DrawCardData()
    {
        if (_drawPile.Count == 0)
            InitializeDeck();

        CardDataSO data = _drawPile[0];
        _drawPile.RemoveAt(0);
        return data;
    }

    public void AddCardToDeck(CardDataSO cardData)
    {
        int insertIndex = Random.Range(0, _drawPile.Count + 1);
        _drawPile.Insert(insertIndex, cardData);
        Debug.Log($"[Deck] Carta agregada: {cardData.name} en posición {insertIndex}/{_drawPile.Count}");
    }

    private void Shuffle(List<CardDataSO> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}