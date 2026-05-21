using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] private Deck deck;
    [SerializeField] private PlayerActions playerActions;
    [SerializeField] private EnemyAI enemyAI;

    [Header("UI")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform[] playerHandContainers;
    [SerializeField] private Transform[] playerPlayedCardContainers;
    [SerializeField] private Transform[] enemyHandContainers;
    [SerializeField] private Transform[] enemyPlayedCardContainers;

    private List<Card> _playerHand = new List<Card>();
    private List<Card> _enemyHand = new List<Card>();
    private int _playerTableIndex = 0;
    private int _enemyTableIndex = 0;

    private void Awake()
    {
        playerActions.OnCardPlayed += OnPlayerCardPlayed;
        enemyAI.OnCardPlayed += OnEnemyCardPlayed;
    }

    private void OnDestroy()
    {
        playerActions.OnCardPlayed -= OnPlayerCardPlayed;
        enemyAI.OnCardPlayed -= OnEnemyCardPlayed;
    }

    public void DealCards()
    {
        ClearTable();
        playerActions.ClearHand();
        enemyAI.ClearHand();

        for (int i = 0; i < 3; i++)
        {
            DrawCard(_playerHand, playerHandContainers[i], true);
            DrawCard(_enemyHand, enemyHandContainers[i], false);
        }
    }

    private void ClearTable()
    {
        foreach (Transform container in playerHandContainers)
        foreach (Transform child in container)
            Destroy(child.gameObject);

        foreach (Transform container in enemyHandContainers)
        foreach (Transform child in container)
            Destroy(child.gameObject);

        foreach (Transform container in playerPlayedCardContainers)
        foreach (Transform child in container)
            Destroy(child.gameObject);

        foreach (Transform container in enemyPlayedCardContainers)
        foreach (Transform child in container)
            Destroy(child.gameObject);

        _playerHand.Clear();
        _enemyHand.Clear();

        _playerTableIndex = 0; 
        _enemyTableIndex = 0;  
    }

    private void DrawCard(List<Card> hand, Transform container, bool isPlayer)
    {
        Card card = deck.DrawCard();
        hand.Add(card);

        GameObject cardGO = Instantiate(cardPrefab, container);
        CardView view = cardGO.GetComponent<CardView>();
        card.cardGO = cardGO;
        view.Setup(card);

        if (isPlayer)
        {
            cardGO.layer = (int)Layers.Player;
            playerActions.AddCard(card);
        }
        else
        {
            cardGO.layer = (int)Layers.None;
            view.Flip(card);
            enemyAI.AddCard(card);
        }
    }

    private void OnPlayerCardPlayed(Card card)
    {
        if (_playerTableIndex >= playerPlayedCardContainers.Length) return;
        MoveCardToTable(card, playerPlayedCardContainers[_playerTableIndex]);
        _playerHand.Remove(card);
        _playerTableIndex++;
    }

    private void OnEnemyCardPlayed(Card card)
    {
        if (_enemyTableIndex >= enemyPlayedCardContainers.Length) return; 
        MoveCardToTable(card, enemyPlayedCardContainers[_enemyTableIndex]);
        _enemyHand.Remove(card);
        _enemyTableIndex++;
    }

    private void MoveCardToTable(Card card, Transform slot)
    {
        card.cardGO.transform.SetParent(slot);
        card.cardGO.transform.localPosition = Vector3.zero;
        card.cardGO.transform.localRotation = Quaternion.identity;
        card.cardGO.layer = (int)Layers.None;
    }
}