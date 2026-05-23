using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    public List<Card> playerHand = new List<Card>();
    public event Action<Card> OnCardPlayed;

    private bool _isPause;

    private GameManager _gm;
    private CardDataSO _pendingSwapCard = null;

    private bool _isSwapMode = false;
    private void Start()
    {
        RunManager.Instance.PauseManager.OnChangePause += OnChangePause_ChangePause;
    }
    private void Update()
    {
        if (_gm == null) return;
        if (_gm.CurrentState != GameState.PlayerTurn) return;
        if (_gm.CurrentCall != CallType.None) return;
        if (_isPause) return;
        if (!Input.GetMouseButtonDown(0)) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider == null) return;
        if (hit.collider.gameObject.layer != (int)Layers.Player) return;
        if (!hit.collider.TryGetComponent<CardView>(out var cardView)) return;

        if (_isSwapMode)
        {
            // Exchange mode: the clicked card is exchanged for the purchased one
            ExecuteSwap(cardView.card);
        }
        else
        {
            // Normal mode: the clicked card is played
            if (_gm.CurrentState != GameState.PlayerTurn) return;

            cardView.SetSelected(true);
            playerHand.Remove(cardView.card);
            OnCardPlayed?.Invoke(cardView.card);
        }
    }

    private void OnDestroy()
    {
        RunManager.Instance.PauseManager.OnChangePause -= OnChangePause_ChangePause;
    }

    public void Initialize(GameManager gm) => _gm = gm;

    public void AddCard(Card card) => playerHand.Add(card);

    public void ClearHand() => playerHand.Clear();
    public void SetSwapMode(bool active, CardDataSO cardData)
    {
        _isSwapMode = active;
        _pendingSwapCard = cardData;
        Debug.Log($"[PlayerActions] Swap mode: {active}");
    }
    public void ReplaceCard(Card oldCard, Card newCard)
    {
        int idx = playerHand.IndexOf(oldCard);
        if (idx >= 0)
            playerHand[idx] = newCard;
    }
    private void ExecuteSwap(Card cardToReplace)
    {
        // Pedimos a Hand que haga el swap físico
        bool success = RunManager.Instance.GameManager
            .GetComponent<Hand>()
            .SwapPlayerCard(cardToReplace, _pendingSwapCard);

        if (success)
        {
            // Limpiamos el estado de swap
            _isSwapMode = false;
            _pendingSwapCard = null;
            RunManager.Instance.CardSwapManager.ClearPendingCard();
        }
    }
    private void OnChangePause_ChangePause(bool isPause) => _isPause = isPause;
}