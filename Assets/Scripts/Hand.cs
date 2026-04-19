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
    private int playerTableIndex = 0;
    private int enemyTableIndex = 0;

    public static event Action<Card> onPlayerHandDealt;
    public static event Action<Card> onEnemyHandDealt;

    private void Awake()
    {
        PlayerActions.onPlayerCardPlayed += PlayerActions_onPlayerCardPlayed;
        EnemyAI.onEnemyCardPlayed += EnemyAI_onEnemyCardPlayed;
    }

    void Start()
    {
        DealCards();
    }

    private void OnDestroy()
    {
        PlayerActions.onPlayerCardPlayed -= PlayerActions_onPlayerCardPlayed;
    }
    private void EnemyAI_onEnemyCardPlayed(Card card)
    {
        if (enemyTableIndex >= enemyHandContainers.Length)
        {
            Debug.LogWarning("No hay más slots de enemigo");
            return;
        }

        Transform slot = enemyHandContainers[enemyTableIndex];

        MoveCardToTable(card, slot);

        enemyHand.Remove(card);

        enemyTableIndex++;
    }

    private void PlayerActions_onPlayerCardPlayed(Card card)
    {
        if (playerTableIndex >= playerHandContainers.Length)
        {
            Debug.LogWarning("No hay más slots de jugador");
            return;
        }

        Transform slot = playerHandContainers[playerTableIndex];

        MoveCardToTable(card, slot);

        playerHand.Remove(card);

        playerTableIndex++;
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
        card.cardGO = cardGO;
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

    private void MoveCardToTable(Card card, Transform slot)
    {
        GameObject cardGO = card.cardGO;

        cardGO.transform.SetParent(slot);
        cardGO.transform.localPosition = Vector3.zero;
        cardGO.transform.localRotation = Quaternion.identity;

        cardGO.layer = (int)Layers.None;
    }

    public void ResetTable()
    {
        playerTableIndex = 0;
        enemyTableIndex = 0;
    }
}