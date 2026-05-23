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

    private void ClearContainer(Transform container)
    {
        for (int i = container.childCount - 1; i >= 0; i--)
            DestroyImmediate(container.GetChild(i).gameObject);
    }

    private void ClearTable()
    {
        foreach (Transform container in playerHandContainers)
            ClearContainer(container);
        foreach (Transform container in enemyHandContainers)
            ClearContainer(container);
        foreach (Transform container in playerPlayedCardContainers)
            ClearContainer(container);
        foreach (Transform container in enemyPlayedCardContainers)
            ClearContainer(container);

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

        if (isPlayer)
            cardGO.layer = (int)Layers.Player;
        else
        {
            cardGO.layer = (int)Layers.None;
            view.Flip(card);
        }

        return card;
    }
    
    private void OnPlayerCardPlayed(Card card)
    {
        if (RunManager.Instance.GameManager.CurrentState != GameState.PlayerTurn) return; 
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
        // Buscar en qué slot visual está la carta
        foreach (Transform container in playerHandContainers)
        {
            CardView view = container.GetComponentInChildren<CardView>();
            if (view != null && view.card == oldCard)
            {
                // Crear la nueva carta con el mismo GameObject
                Card newCard = new Card(newCardData)
                {
                    cardGO = oldCard.cardGO
                };

                // Actualizar el visual sin cambiar el GameObject
                view.Setup(newCard);
                newCard.cardGO.layer = (int)Layers.Player;

                // Actualizar la lista interna de Hand
                int idx = _playerHand.IndexOf(oldCard);
                if (idx >= 0) _playerHand[idx] = newCard;

                // Actualizar la lista en PlayerActions
                playerActions.ReplaceCard(oldCard, newCard);

                // Descartar la carta vieja al mazo
                deck.Discard(oldCard);

                return true;
            }
        }
        return false;
    }
}