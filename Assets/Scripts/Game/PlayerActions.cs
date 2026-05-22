using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    public List<Card> playerHand = new List<Card>();
    public event Action<Card> OnCardPlayed;

    private bool _isPause;

    private GameManager _gm;

    private void OnEnable()
    {
        PauseManager.Instance.OnChangePause += OnChangePause_ChangePause;
    }

    private void Update()
    {
        if (_gm.CurrentState != GameState.PlayerTurn) return;

        if (!_isPause && Input.GetMouseButtonDown(0))
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

    private void OnDisable()
    {
        PauseManager.Instance.OnChangePause -= OnChangePause_ChangePause;
    }

    public void Initialize(GameManager gm) => _gm = gm;

    public void AddCard(Card card) => playerHand.Add(card);

    public void ClearHand() => playerHand.Clear();

    private void OnChangePause_ChangePause(bool isPause) => _isPause = isPause;
}