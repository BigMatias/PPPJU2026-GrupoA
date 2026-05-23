using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private List<CardDataSO> startingDeck;

    private List<CardDataSO> drawPile = new List<CardDataSO>();
    private List<CardDataSO> discardPile = new List<CardDataSO>();

    private void Awake()
    {
        InitializeDeck();
        Shuffle(drawPile);
    }

    private void InitializeDeck()
    {
        drawPile.Clear();
        foreach (var cardData in startingDeck)
            drawPile.Add(cardData);
    }

    private void Reshuffle()
    {
        drawPile.AddRange(discardPile);
        discardPile.Clear();
        Shuffle(drawPile);
    }

    public void Shuffle(List<CardDataSO> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    public CardDataSO DrawCardData()
    {
        if (drawPile.Count == 0)
            Reshuffle();

        CardDataSO data = drawPile[0];
        drawPile.RemoveAt(0);
        return data;
    }

    public void Discard(Card card)
    {
        discardPile.Add(card.cardDataSO);
    }
    
    public void AddCardToDeck(CardDataSO cardData)
    {
        Card newCard = new Card(cardData);
        drawPile.Add(newCard);
        Debug.Log($"[Deck] Carta agregada: {cardData.name}");
    }
}