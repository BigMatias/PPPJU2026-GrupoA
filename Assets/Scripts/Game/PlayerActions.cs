using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    public List<Card> playerHand = new List<Card>();
    public event Action<Card> OnCardPlayed;

    private GameManager _gm;
    private bool _isPaused;

    private void Start()
    {
        _gm = RunManager.Instance.GameManager;
    }

    private void OnEnable()
    {
        PauseManager.Instance.OnChangePause += OnPauseChanged;
    }

    private void OnDisable()
    {
        PauseManager.Instance.OnChangePause -= OnPauseChanged;
    }

    private void Update()
    {
        if (_gm == null) return;
        if (_gm.CurrentState != GameState.PlayerTurn) return;
        if (_isPaused) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject.layer == (int)Layers.Player)
            {
                if (hit.collider.TryGetComponent<CardView>(out var cardView))
                {
                    cardView.SetSelected(true);
                    playerHand.Remove(cardView.card);
                    OnCardPlayed?.Invoke(cardView.card);
                }
            }
        }
    }

    public void AddCard(Card card) => playerHand.Add(card);
    public void ClearHand() => playerHand.Clear();

    private void OnPauseChanged(bool isPaused) => _isPaused = isPaused;
}