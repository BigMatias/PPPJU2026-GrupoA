using System;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] private Deck deck;

    [Header("UI")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameDataSO gameDataSO;
    [SerializeField] private Transform[] playerHandContainers;
    [SerializeField] private Transform[] enemyHandContainers;

    private List<Card> playerHand = new List<Card>();
    private List<Card> enemyHand = new List<Card>();

    public static event Action<Card> onPlayerHandDealt;
    public static event Action<Card> onEnemyHandDealt;

    void Start()
    {
        DealCards();
    }

    private void DealCards()
    {
        for (int i = 0; i < 3; i++)
        {
            DrawCard(playerHand, playerHandContainers[i]);
            DrawCard(enemyHand, enemyHandContainers[i]);
        }
    }

    private void DrawCard(List<Card> hand, Transform container)
    {
        Card card = deck.DrawCard();
        hand.Add(card);

        GameObject cardGO = Instantiate(cardPrefab, container);
        CardView view = cardGO.GetComponent<CardView>();
        view.Setup(card);

        if (hand == playerHand)
        {
            cardGO.layer = (int)Layers.Player;
            onPlayerHandDealt?.Invoke(card);
        }

        if (hand == enemyHand)
        {
            onEnemyHandDealt?.Invoke(card);
            cardGO.layer = (int)Layers.None;
            view.Flip(card);
        }
    }


}