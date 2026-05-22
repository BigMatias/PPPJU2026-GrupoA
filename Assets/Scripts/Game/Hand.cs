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
    
    private List<Card> _playerPlayedCards = new List<Card>();
    private List<Card> _enemyPlayedCards = new List<Card>();
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
        
        foreach (Card card in _playerHand) { card.cardGO = null; deck.Discard(card); }
        foreach (Card card in _enemyHand) { card.cardGO = null; deck.Discard(card); }
        foreach (Card card in _playerPlayedCards) { card.cardGO = null; deck.Discard(card); }
        foreach (Card card in _enemyPlayedCards) { card.cardGO = null; deck.Discard(card); }
        
        _playerPlayedCards.Clear();
        _enemyPlayedCards.Clear();
        _playerHand.Clear();
        _enemyHand.Clear();

        _playerTableIndex = 0; 
        _enemyTableIndex = 0;  
    }

    private void DrawCard(List<Card> hand, Transform container, bool isPlayer)
    {
        Card card = deck.DrawCard();

        GameObject cardGO = Instantiate(cardPrefab, container);
        CardView view = cardGO.GetComponent<CardView>();
        card.cardGO = cardGO;
        
        view.Setup(card);
        hand.Add(card);

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

        Transform slot = playerPlayedCardContainers[_playerTableIndex];
        MoveCardToTable(card, slot);
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
        if (card.cardGO == null)
        {
            return;
        }
        if (slot == null)
        {
            return;
        }
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

                Debug.Log($"[Hand] Swap: {oldCard.cardDataSO.name} → {newCardData.name}");
                return true;
            }
        }
        Debug.LogWarning("[Hand] SwapPlayerCard: no se encontró la carta en los containers.");
        return false;
    }
}