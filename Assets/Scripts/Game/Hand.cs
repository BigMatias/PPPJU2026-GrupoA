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
    private List<Card> _playerPlayedCards = new List<Card>();
    private List<Card> _enemyPlayedCards = new List<Card>();

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

        for (int i = 0; i < 3; i++)
        {
            Card playerCard = CreateCard(playerHandContainers[i], true);
            _playerHand.Add(playerCard);
            playerActions.AddCard(playerCard);

            Card enemyCard = CreateCard(enemyHandContainers[i], false);
            _enemyHand.Add(enemyCard);
            enemyAI.AddCard(enemyCard);
        }
    }

    private void ClearTable()
    {
        for (int i = playerHandContainers.Length - 1; i >= 0; i--)
            for (int j = playerHandContainers[i].childCount - 1; j >= 0; j--)
                DestroyImmediate(playerHandContainers[i].GetChild(j).gameObject);

        for (int i = enemyHandContainers.Length - 1; i >= 0; i--)
            for (int j = enemyHandContainers[i].childCount - 1; j >= 0; j--)
                DestroyImmediate(enemyHandContainers[i].GetChild(j).gameObject);

        for (int i = playerPlayedCardContainers.Length - 1; i >= 0; i--)
            for (int j = playerPlayedCardContainers[i].childCount - 1; j >= 0; j--)
                DestroyImmediate(playerPlayedCardContainers[i].GetChild(j).gameObject);

        for (int i = enemyPlayedCardContainers.Length - 1; i >= 0; i--)
            for (int j = enemyPlayedCardContainers[i].childCount - 1; j >= 0; j--)
                DestroyImmediate(enemyPlayedCardContainers[i].GetChild(j).gameObject);

        _playerHand.Clear();
        _enemyHand.Clear();
        _playerPlayedCards.Clear();
        _enemyPlayedCards.Clear();

        playerActions.ClearHand();
        enemyAI.ClearHand();

        _playerTableIndex = 0;
        _enemyTableIndex = 0;
    }

    private Card CreateCard(Transform container, bool isPlayer)
    {
        CardDataSO data = deck.DrawCardData();
        Card card = new Card(data);

        GameObject cardGO = Instantiate(cardPrefab, container);
        CardView view = cardGO.GetComponent<CardView>();
        card.cardGO = cardGO;
        view.Setup(card);

        cardGO.layer = isPlayer ? (int)Layers.Player : (int)Layers.None;
        if (!isPlayer) view.Flip(card);

        return card;
    }

    private void OnPlayerCardPlayed(Card card)
    {
        if (_playerTableIndex >= playerPlayedCardContainers.Length) return;
        MoveCardToTable(card, playerPlayedCardContainers[_playerTableIndex]);
        _playerHand.Remove(card);
        _playerPlayedCards.Add(card);
        _playerTableIndex++;
    }

    private void OnEnemyCardPlayed(Card card)
    {
        if (_enemyTableIndex >= enemyPlayedCardContainers.Length) return;
        MoveCardToTable(card, enemyPlayedCardContainers[_enemyTableIndex]);
        _enemyHand.Remove(card);
        _enemyPlayedCards.Add(card);
        _enemyTableIndex++;
    }

    private void MoveCardToTable(Card card, Transform slot)
    {
        if (card.cardGO == null || slot == null) return;
        card.cardGO.transform.SetParent(slot);
        card.cardGO.transform.localPosition = Vector3.zero;
        card.cardGO.transform.localRotation = Quaternion.identity;
        card.cardGO.layer = (int)Layers.None;
    }

    public bool SwapPlayerCard(Card oldCard, CardDataSO newCardData)
    {
        foreach (Transform container in playerHandContainers)
        {
            CardView view = container.GetComponentInChildren<CardView>();
            if (view != null && view.card == oldCard)
            {
                Card newCard = new Card(newCardData) { cardGO = oldCard.cardGO };
                view.Setup(newCard);
                newCard.cardGO.layer = (int)Layers.Player;

                int idx = _playerHand.IndexOf(oldCard);
                if (idx >= 0) _playerHand[idx] = newCard;

                playerActions.ReplaceCard(oldCard, newCard);
                return true;
            }
        }
        return false;
    }
}