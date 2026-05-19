using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    public List<Card> playerHand = new List<Card>();
    public event Action<Card> OnCardPlayed;

    private GameManager _gm;

    public void Initialize(GameManager gm)
    {
        _gm = gm;
    }

    public void AddCard(Card card) => playerHand.Add(card);

    public void ClearHand() => playerHand.Clear();

    private void Update()
    {
        if (_gm.CurrentState != GameState.PlayerTurn) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject.layer == (int)Layers.Player)
            {
                CardView cardView = hit.collider.GetComponent<CardView>();
                if (cardView != null)
                {
                    cardView.SetSelected(true);
                    playerHand.Remove(cardView.card);
                    OnCardPlayed?.Invoke(cardView.card);
                }
            }
        }
    }
}