using UnityEngine;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Hand hand;
    public List<Card> enemyHand = new List<Card>();

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

        int decision = Random.Range(0, 100);

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

    //  JUGAR CARTA
    void PlayCard(GameManager gm)
    {
        Card card = enemyHand[0];
        enemyHand.RemoveAt(0);

        Debug.Log("IA juega carta: " + card.cardDataSO.value + " de " + card.cardDataSO.suit);

        hand.PlayEnemyCard(card);
        CardView cardView = card.cardGO.GetComponent<CardView>();
        cardView.Flip(card);

        gm.EndEnemyTurn();
    }

    //  TRUCO
    void SingTruco(GameManager gm)
    {
        if (gm.trucoState == TrucoState.None)
        {
            gm.trucoState = TrucoState.Truco;
            Debug.Log("IA canta TRUCO");
        }

        gm.WaitPlayerResponse();
    }

    void RespondToTruco(GameManager gm)
    {
        int decision = Random.Range(0, 100);
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
            decision = Random.Range(0, 100);

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

    void RaiseTruco(GameManager gm)
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

    //  ENVIDO
    void SingEnvido(GameManager gm)
    {
        int decision = Random.Range(0, 3);

        if (decision == 0)
        {
            gm.envidoState = EnvidoState.Envido;
            Debug.Log("IA canta ENVIDO");
        }
        else if (decision == 1)
        {
            gm.envidoState = EnvidoState.RealEnvido;
            Debug.Log("IA canta REAL ENVIDO");
        }
        else
        {
            gm.envidoState = EnvidoState.FaltaEnvido;
            Debug.Log("IA canta FALTA ENVIDO");
        }

        gm.WaitPlayerResponse();
    }

    void RespondToEnvido(GameManager gm)
    {
        gm.currentCall = CallType.Envido; 

        int decision = Random.Range(0, 100);

        if (decision < 30)
        {
            Debug.Log("IA no quiere envido");

            gm.envidoResolved = true;
            gm.ResolveCall();
            gm.EndEnemyTurn();
            return;
        }

        if (decision < 60)
        {
            Debug.Log("IA acepta envido");

            gm.ResolveEnvido();
            gm.EndEnemyTurn();
            return;
        }

        if (gm.envidoState == EnvidoState.Envido)
        {
            int subDecision = Random.Range(0, 100);

            if (subDecision < 33)
            {
                Debug.Log("IA canta ENVIDO ENVIDO");
                gm.envidoState = EnvidoState.EnvidoEnvido;
            }
            else if (subDecision < 66)
            {
                Debug.Log("IA canta REAL ENVIDO");
                gm.envidoState = EnvidoState.RealEnvido;
            }
            else
            {
                Debug.Log("IA canta FALTA ENVIDO");
                gm.envidoState = EnvidoState.FaltaEnvido;
            }
        }
        else if (gm.envidoState == EnvidoState.EnvidoEnvido)
        {
            gm.envidoState = (Random.value < 0.5f)
                ? EnvidoState.RealEnvido
                : EnvidoState.FaltaEnvido;
        }
        else if (gm.envidoState == EnvidoState.RealEnvido)
        {
            gm.envidoState = EnvidoState.FaltaEnvido;
        }

        gm.callOwner = CallOwner.Enemy;
        gm.WaitPlayerResponse();
    }

    void Fold(GameManager gm)
    {
        Debug.Log("IA se fue al mazo");
        gm.ResolveCall();
        gm.EndRound();
    }
}