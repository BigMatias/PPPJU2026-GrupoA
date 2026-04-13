using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    public List<Card> playerHand = new List<Card>();
    public static event Action onCardPlayed;

    private void Awake()
    {
        Hand.onPlayerHandDealt += Hand_onPlayerHandDealt;
    }

    private void Update()
    {
        if (GameManager.Instance.currentState != GameState.PlayerTurn)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10);

            RaycastHit2D hit = Physics2D.Raycast(mousePos, direction);

            if (hit.collider != null && hit.collider.gameObject.layer == (int)Layers.Player)
            {
                CardView card = hit.collider.GetComponent<CardView>();
                if (card != null)
                {
                    card.SetSelected(true);
                    onCardPlayed?.Invoke();
                }
            }
        }
    }

    private void OnDestroy()
    {
        Hand.onPlayerHandDealt -= Hand_onPlayerHandDealt;
    }

    private void Hand_onPlayerHandDealt(Card card)
    {
        playerHand.Add(card);
    }
}
