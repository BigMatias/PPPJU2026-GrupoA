using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public List<Card> enemyHand = new List<Card>();
    public event Action<Card> OnCardPlayed;

    public void AddCard(Card card) => enemyHand.Add(card);
    public void ClearHand() => enemyHand.Clear();

    public void RespondToPlayer(GameManager gm)
    {
        if (gm.CurrentCall == CallType.Truco) { RespondToTruco(gm); return; }
        if (gm.CurrentCall == CallType.Envido) { RespondToEnvido(gm); return; }
        TakeTurn(gm);
    }

    // ── Turno libre ────────────────────────────────────────────────

    private void TakeTurn(GameManager gm)
    {
        int decision = UnityEngine.Random.Range(0, 100);

        if (decision < 40)
            PlayCard();
        else if (decision < 60 && !gm.TrucoPlayedThisRound)
            gm.EnemySingsTruco();
        else if (decision < 80 && !gm.EnvidoResolved && gm.IsFirstRound())
            SingEnvido(gm);
        else
            PlayCard();
    }

    // ── Truco ──────────────────────────────────────────────────────

    private void RespondToTruco(GameManager gm)
    {
        int decision = UnityEngine.Random.Range(0, 100);

        if (decision < 30) { gm.EnemyFolds(); return; }
        if (decision < 70) { gm.EnemyAccepts(); return; }

        // subir solo si puede
        if (gm.CallOwner == CallOwner.Player && gm.TrucoState != TrucoState.ValeCuatro)
            gm.EnemyRaisesTruco();
        else
            gm.EnemyAccepts();
    }

    // ── Envido ─────────────────────────────────────────────────────

    private void RespondToEnvido(GameManager gm)
    {
        int decision = UnityEngine.Random.Range(0, 100);

        if (decision < 30) { gm.EnemyDenies(); return; }
        if (decision < 70) { gm.EnemyAccepts(); return; }

        // subir si puede
        EnvidoState raise = GetEnvidoRaise(gm.EnvidoState);
        if (raise != EnvidoState.None && gm.CallOwner == CallOwner.Player)
            gm.EnemySingsEnvido(raise);
        else
            gm.EnemyAccepts();
    }

    private EnvidoState GetEnvidoRaise(EnvidoState current) => current switch
    {
        EnvidoState.None => EnvidoState.Envido,
        EnvidoState.Envido => UnityEngine.Random.value < 0.5f ? EnvidoState.EnvidoEnvido : EnvidoState.RealEnvido,
        EnvidoState.EnvidoEnvido => EnvidoState.RealEnvido,
        EnvidoState.RealEnvido => EnvidoState.FaltaEnvido,
        _ => EnvidoState.None
    };

    // ── Carta ──────────────────────────────────────────────────────

    private void SingEnvido(GameManager gm)
    {
        EnvidoState[] options = { EnvidoState.Envido, EnvidoState.RealEnvido, EnvidoState.FaltaEnvido };
        gm.EnemySingsEnvido(options[UnityEngine.Random.Range(0, options.Length)]);
    }

    private void PlayCard()
    {
        if (enemyHand.Count == 0) return;
        Card card = enemyHand[0];
        enemyHand.RemoveAt(0);
        CardView view = card.cardGO.GetComponent<CardView>();
        view.Flip(card);
        view.SetSelected();
        OnCardPlayed?.Invoke(card);
    }
}