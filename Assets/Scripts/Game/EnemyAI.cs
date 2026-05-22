using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public List<Card> enemyHand = new List<Card>();
    public event Action<Card> OnCardPlayed;

    public void AddCard(Card card)
    {
        enemyHand.Add(card);
    }

    public void ClearHand()
    {
        enemyHand.Clear();
    }

    public void RespondToPlayer(GameManager gm)
    {
        int decision = UnityEngine.Random.Range(0, 100);

        if (gm.CurrentCall == CallType.Truco) { RespondToTruco(gm); return; }
        if (gm.CurrentCall == CallType.Envido) { RespondToEnvido(gm); return; }

        if (decision < 40) PlayCard(gm);
        else if (decision < 60 && !gm.TrucoPlayedThisRound) gm.EnemySingsTruco();
        else if (decision < 80 && !gm.EnvidoResolved && gm.IsFirstRound()) SingEnvido(gm);
        else PlayCard(gm);
    }

    private void PlayCard(GameManager gm)
    {
        if (enemyHand.Count == 0) return;
        Card card = enemyHand[0];
        if (card.cardGO == null)
        {
            enemyHand.RemoveAt(0); 
            return;
        }
        enemyHand.RemoveAt(0);
        card.cardGO.GetComponent<CardView>().Flip(card);
        OnCardPlayed?.Invoke(card);
    }

    private void SingEnvido(GameManager gm)
    {
        EnvidoState[] options = { EnvidoState.Envido, EnvidoState.RealEnvido, EnvidoState.FaltaEnvido };
        gm.EnemySingsEnvido(options[UnityEngine.Random.Range(0, options.Length)]);
    }

    private void RespondToTruco(GameManager gm)
    {
        int decision = UnityEngine.Random.Range(0, 100);

        if (decision < 30) { gm.EnemyFolds(); return; }
        if (decision < 60) { gm.EnemyAccepts(); return; }

        if (gm.CallOwner == CallOwner.Player && gm.TrucoState != TrucoState.ValeCuatro)
        {
            gm.EnemyRaisesTruco();
            return;
        }

        if (UnityEngine.Random.value < 0.5f) gm.EnemyFolds();
        else gm.EnemyAccepts();
    }

    private void RespondToEnvido(GameManager gm)
    {
        int decision = UnityEngine.Random.Range(0, 100);

        if (decision < 30) { gm.EnemyDenies(); return; }
        if (decision < 60) { gm.EnemyAccepts(); return; }

        EnvidoState raised = gm.EnvidoState switch
        {
            EnvidoState.Envido => RandomEnvidoRaise(),
            EnvidoState.EnvidoEnvido => UnityEngine.Random.value < 0.5f 
                ? EnvidoState.RealEnvido 
                : EnvidoState.FaltaEnvido,
            EnvidoState.RealEnvido => EnvidoState.FaltaEnvido,
            _ => EnvidoState.None
        };

        if (raised != EnvidoState.None) gm.EnemySingsEnvido(raised);
        else gm.EnemyAccepts();
    }

    private EnvidoState RandomEnvidoRaise()
    {
        return UnityEngine.Random.Range(0, 3) switch
        {
            0 => EnvidoState.EnvidoEnvido,
            1 => EnvidoState.RealEnvido,
            _ => EnvidoState.FaltaEnvido
        };
    }
}