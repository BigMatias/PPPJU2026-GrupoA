using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    public List<Card> playerHand = new List<Card>();
    public event Action<Card> OnCardPlayed;

    private GameManager _gm;
    private bool _isPaused;
    private bool _isSwapMode;
    private CardDataSO _pendingSwapCard;

    private void Start()
    {
        _gm = RunManager.Instance.GameManager;
        RunManager.Instance.PauseManager.OnChangePause += OnPauseChanged;
    }

    private void OnDisable()
    {
        RunManager.Instance.PauseManager.OnChangePause -= OnPauseChanged;
    }

    private void Update()
    {
        if (_gm == null) return;
        if (_isPaused) return;
        if (!Input.GetMouseButtonDown(0)) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider == null) return;
        if (hit.collider.gameObject.layer != (int)Layers.Player) return;
        if (!hit.collider.TryGetComponent<CardView>(out var cardView)) return;

        if (_isSwapMode)
        {
            ExecuteSwap(cardView.card);
        }
        else
        {
            if (_gm.CurrentState != GameState.PlayerTurn) return;
            if (_gm.CurrentCall != CallType.None) return;

            cardView.SetSelected();
            playerHand.Remove(cardView.card);
            OnCardPlayed?.Invoke(cardView.card);
        }
    }

    public void AddCard(Card card) => playerHand.Add(card);
    public void ClearHand() => playerHand.Clear();

    public void ReplaceCard(Card oldCard, Card newCard)
    {
        int idx = playerHand.IndexOf(oldCard);
        if (idx >= 0) playerHand[idx] = newCard;
    }

    public void SetSwapMode(bool active, CardDataSO pendingCard)
    {
        _isSwapMode = active;
        _pendingSwapCard = pendingCard;
    }

    private void ExecuteSwap(Card cardToReplace)
    {
        if (_pendingSwapCard == null) return;

        Hand hand = RunManager.Instance.GameManager.GetComponent<Hand>();
        if (hand == null)
            hand = FindObjectOfType<Hand>();

        bool success = hand.SwapPlayerCard(cardToReplace, _pendingSwapCard);
        if (success)
        {
            RunManager.Instance.CardSwapManager.ClearPendingCard();
            _isSwapMode = false;
            _pendingSwapCard = null;
        }
    }

    private void OnPauseChanged(bool isPaused) => _isPaused = isPaused;
}