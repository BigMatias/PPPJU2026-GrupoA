using UnityEngine;
using System.Collections.Generic;
using System;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Hand hand;
    public List<Card> enemyHand = new List<Card>();
    public static event Action<Card> onEnemyCardPlayed;  

    private void Awake()
    {
        Hand.onEnemyHandDealt += Hand_onEnemyHandDealt;
    }

    private void OnDestroy()
    {
        Hand.onEnemyHandDealt -= Hand_onEnemyHandDealt;
    }

    private void Hand_onEnemyHandDealt(Card card)
    {
        enemyHand.Add(card);
    }

    public void RespondToPlayer(GameManager gm)
    {
        Debug.Log("IA pensando...");

        int decision = UnityEngine.Random.Range(0, 100);

        if (gm.trucoState != TrucoState.None)
        {
            RespondToTruco(gm);
            return;
        }

        if (gm.envidoState != EnvidoState.None)
        {
            RespondToEnvido(gm);
            return;
        }

        if (decision < 40)
        {
            PlayCard(gm);
        }
        else if (decision < 60 && !gm.trucoPlayedThisRound)
        {
            SingTruco(gm);
        }
        else if (decision < 80 && !gm.envidoResolved && gm.IsFirstRound())
        {
            SingEnvido(gm);
        }
        else
        {
            Fold(gm);
        }
    }

    private void PlayCard(GameManager gm)
    {
        Card card = enemyHand[0];
        enemyHand.RemoveAt(0);

        Debug.Log("IA juega carta: " + card.cardDataSO.value + " de " + card.cardDataSO.suit);

        onEnemyCardPlayed?.Invoke(card);
        CardView cardView = card.cardGO.GetComponent<CardView>();
        cardView.Flip(card);

        gm.EndEnemyTurn();
    }

    private void SingTruco(GameManager gm)
    {
        if (gm.trucoState == TrucoState.None)
        {
            gm.trucoState = TrucoState.Truco;
            gm.currentCall = CallType.Truco;
            gm.callOwner = CallOwner.Enemy;
            Debug.Log("IA canta TRUCO");
        }

        gm.WaitPlayerResponse();
    }

    private void RespondToTruco(GameManager gm)
    {
        int decision = UnityEngine.Random.Range(0, 100);
        Debug.Log(decision);
        if (decision < 30)
        {
            Debug.Log("IA no quiere -> se va al mazo");
            Fold(gm);
        }
        else if (decision < 60)
        {
            Debug.Log("IA acepta truco");
            gm.ResolveTruco();
            gm.EndEnemyTurn();
        }
        else if (decision > 60 && gm.callOwner == CallOwner.Player && gm.trucoState != TrucoState.ValeCuatro)
        {
            RaiseTruco(gm);
        }
        else
        {
            decision = UnityEngine.Random.Range(0, 100);

            if (decision < 50)
            {
                Debug.Log("IA no quiere, se va al mazo");
                Fold(gm);
            }
            else
            {
                Debug.Log("IA acepta truco");
                gm.ResolveTruco();
                gm.EndEnemyTurn();
            }
        }
    }

    private void RaiseTruco(GameManager gm)
    {
        if (gm.trucoState == TrucoState.Truco)
        {
            gm.trucoState = TrucoState.Retruco;
            gm.callOwner = CallOwner.Enemy;
            Debug.Log("IA canta RETRUCO");
        }
        else if (gm.trucoState == TrucoState.Retruco)
        {
            gm.trucoState = TrucoState.ValeCuatro;
            gm.callOwner = CallOwner.Enemy;
            Debug.Log("IA canta VALE CUATRO");
        }

        gm.WaitPlayerResponse();
    }

    private void SingEnvido(GameManager gm)
    {
        int decision = UnityEngine.Random.Range(0, 3);

        if (decision == 0)
        {
            gm.envidoState = EnvidoState.Envido;
            gm.callOwner = CallOwner.Enemy;
            Debug.Log("IA canta ENVIDO");
        }
        else if (decision == 1)
        {
            gm.envidoState = EnvidoState.RealEnvido;
            gm.callOwner = CallOwner.Enemy;
            Debug.Log("IA canta REAL ENVIDO");
        }
        else
        {
            gm.envidoState = EnvidoState.FaltaEnvido;
            gm.callOwner = CallOwner.Enemy;
            Debug.Log("IA canta FALTA ENVIDO");
        }
        gm.currentCall = CallType.Envido;
        gm.WaitPlayerResponse();
    }

    private void RespondToEnvido(GameManager gm)
    {
        bool envidoSettled;
        gm.currentCall = CallType.Envido; 

        int decision = UnityEngine.Random.Range(0, 100);

        if (decision < 30)
        {
            Debug.Log("IA no quiere envido");

            gm.CallDenied();
            gm.EndEnemyTurn();
            return;
        }

        if (decision < 60)
        {
            Debug.Log("IA acepta envido");
            envidoSettled = true;
            gm.EnvidoManager(envidoSettled);
            gm.EndEnemyTurn();
            return;
        }

        if (gm.envidoState == EnvidoState.Envido)
        {
            int subDecision = UnityEngine.Random.Range(0, 100);

            if (subDecision < 33)
            {
                Debug.Log("IA canta ENVIDO ENVIDO");
                gm.envidoState = EnvidoState.EnvidoEnvido;
                envidoSettled = false;
                gm.EnvidoManager(envidoSettled);
            }
            else if (subDecision < 66)
            {
                Debug.Log("IA canta REAL ENVIDO");
                gm.envidoState = EnvidoState.RealEnvido;
                envidoSettled = false;
                gm.EnvidoManager(envidoSettled);
            }
            else
            {
                Debug.Log("IA canta FALTA ENVIDO");
                gm.envidoState = EnvidoState.FaltaEnvido;
                envidoSettled = false;
                gm.EnvidoManager(envidoSettled);
            }
            gm.callOwner = CallOwner.Enemy;
        }
        else if (gm.envidoState == EnvidoState.EnvidoEnvido)
        {
            gm.envidoState = (UnityEngine.Random.value < 0.5f)
                ? EnvidoState.RealEnvido
                : EnvidoState.FaltaEnvido;
            gm.callOwner = CallOwner.Enemy;
        }
        else if (gm.envidoState == EnvidoState.RealEnvido)
        {
            gm.envidoState = EnvidoState.FaltaEnvido;
            gm.callOwner = CallOwner.Enemy;
        }

        gm.WaitPlayerResponse();
    }

    private void Fold(GameManager gm)
    {
        Debug.Log("IA se fue al mazo");
        gm.CallDenied();
        bool playerWonHand = true;
        gm.EndRound(playerWonHand);
    }
}