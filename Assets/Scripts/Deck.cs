using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private List<CardDataSO> startingDeck; 

    private List<Card> drawPile = new List<Card>();
    private List<Card> discardPile = new List<Card>();

    void Start()
    {
        InitializeDeck();
        Shuffle(drawPile);
    }

    void InitializeDeck()
    {
        drawPile.Clear();

        foreach (var cardData in startingDeck)
        {
            drawPile.Add(new Card(cardData));
        }
    }

    public Card DrawCard()
    {
        if (drawPile.Count == 0)
        {
            Reshuffle();
        }

        Card card = drawPile[0];
        drawPile.RemoveAt(0);

        return card;
    }

    void Reshuffle()
    {
        drawPile.AddRange(discardPile);
        discardPile.Clear();
        Shuffle(drawPile);
    }

    void Shuffle(List<Card> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    public void Discard(Card card)
    {
        discardPile.Add(card);
    }
}