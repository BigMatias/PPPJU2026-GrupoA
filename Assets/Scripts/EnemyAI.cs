using UnityEngine;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    public List<Card> enemyHand = new List<Card>();

    private void Awake()
    {
        Hand.onEnemyHandDealt += Hand_onEnemyHandDealt;
    }

    private void Start()
    {
        
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
        else if (decision < 60)
        {
            SingTruco(gm);
        }
        else if (decision < 80)
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

        if (decision < 30)
        {
            Debug.Log("IA no quiere -> se va al mazo");
            Fold(gm);
        }
        else if (decision < 60)
        {
            Debug.Log("IA acepta truco");
            gm.trucoState = TrucoState.None;
            PlayCard(gm);
        }
        else
        {
            RaiseTruco(gm);
        }
    }

    void RaiseTruco(GameManager gm)
    {
        if (gm.trucoState == TrucoState.Truco)
        {
            gm.trucoState = TrucoState.Retruco;
            Debug.Log("IA canta RETRUCO");
        }
        else if (gm.trucoState == TrucoState.Retruco)
        {
            gm.trucoState = TrucoState.ValeCuatro;
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
        int decision = Random.Range(0, 100);

        if (decision < 30)
        {
            Debug.Log("IA no quiere envido");
            gm.envidoState = EnvidoState.None;
            gm.EndEnemyTurn();
        }
        else if (decision < 60)
        {
            Debug.Log("IA acepta envido");
            gm.envidoState = EnvidoState.None;
            gm.EndEnemyTurn();
        }
        else
        {
            Debug.Log("IA redobla envido");

            gm.envidoState = EnvidoState.FaltaEnvido;
            gm.WaitPlayerResponse();
        }
    }

    //  IRSE AL MAZO
    void Fold(GameManager gm)
    {
        Debug.Log("IA se fue al mazo");
        gm.EndRound();
    }
}