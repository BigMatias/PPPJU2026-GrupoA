using System;
using UnityEngine;
public class CardSwapManager : MonoBehaviour
{
    public event Action<CardDataSO> OnPendingCardSet;
    public event Action OnPendingCardCleared;

    public CardDataSO PendingCard { get; private set; }
    public bool HasPendingCard => PendingCard != null;
    public void SetPendingCard(CardDataSO cardData)
    {
        PendingCard = cardData;
        OnPendingCardSet?.Invoke(cardData);
    }
    public void ClearPendingCard()
    {
        PendingCard = null;
        OnPendingCardCleared?.Invoke();
    }
}