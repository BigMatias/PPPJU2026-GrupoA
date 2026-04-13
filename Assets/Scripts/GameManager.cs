using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action<int> OnWinEnvido;
    public static event Action<TrucoState> OnWinTruco;
    public static event Action<int> OnWinRound;

    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private GameDataSO gameDataSO;
    [SerializeField] private RunDataSO runDataSO;
    [SerializeField] private PlayerActions playerActions;

    public static GameManager Instance;
    public TrucoState trucoState = TrucoState.None;
    public EnvidoState envidoState = EnvidoState.None;
    public GameState currentState;
    public CallType currentCall = CallType.None;
    public CallOwner callOwner = CallOwner.None;

    public bool trucoPlayedThisRound = false;
    public bool envidoResolved = false;
    private int currentRound = 0;
    private int envidoPoints;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameDataSO.trucoPoints = 0;
        gameDataSO.envidoPoints = 0;
        currentState = GameState.PlayerTurn;
    }

    private void OnEnable()
    {
        PlayerActions.onCardPlayed += OnPlayerCardPlayed;
    }

    private void OnDisable()
    {
        PlayerActions.onCardPlayed -= OnPlayerCardPlayed;
    }

    private void OnPlayerCardPlayed()
    {
        if (currentState != GameState.PlayerTurn)
            return;

        currentState = GameState.WaitingEnemyResponse;

        Invoke(nameof(EnemyTurn), 1.5f);
    }

    public void ResolveCall()
    {
        if (currentCall == CallType.Envido)
        {
            switch (envidoState)
            {
                case EnvidoState.Envido:
                    {
                        gameDataSO.trucoPoints += 1;
                        break;
                    }
                case EnvidoState.EnvidoEnvido:
                    {
                        gameDataSO.trucoPoints += 2;
                        break;
                    }
                case EnvidoState.RealEnvido:
                    {
                        gameDataSO.trucoPoints += 4;
                        break;
                    }
                case EnvidoState.FaltaEnvido:
                    {
                        //Se debería guardar el envido anterior y dar esa cantidad de puntos
                        break;
                    }
            }
            gameDataSO.envidoPoints += 1;
            envidoState = EnvidoState.None;
            envidoResolved = true;
        }
        if (currentCall == CallType.Truco)
        {
            switch (trucoState)
            {
                case TrucoState.Truco:
                    {
                        gameDataSO.trucoPoints += 1;
                        break;
                    }
                case TrucoState.Retruco:
                    {
                        gameDataSO.trucoPoints += 2;
                        break;
                    }
                case TrucoState.ValeCuatro:
                    {
                        gameDataSO.trucoPoints += 3;
                        break;
                    }
            }
            
        }
        currentCall = CallType.None;
        callOwner = CallOwner.None;

    }

    public void ResolveEnvido()
    {
        Debug.Log("Resolviendo ENVIDO");

        int playerPoints = CalculateEnvido(playerActions.playerHand);
        int enemyPoints = CalculateEnvido(enemyAI.enemyHand);

        Debug.Log("Jugador: " + playerPoints);
        Debug.Log("Enemigo: " + enemyPoints);

        switch (envidoState)
        {
            case EnvidoState.Envido:
                {
                    envidoPoints = runDataSO.envidoPointsStat;
                    break;
                }
            case EnvidoState.EnvidoEnvido:
                {
                    envidoPoints = runDataSO.envidoEnvidoPointsStat;
                    break;
                }
            case EnvidoState.RealEnvido:
                {
                    envidoPoints = runDataSO.realEnvidoPointsStat;
                    break;
                }
            case EnvidoState.FaltaEnvido:
                {
                    envidoPoints = runDataSO.faltaEnvidoPointsStat;
                    break;
                }
        }
        if (playerPoints > enemyPoints)
        {
            Debug.Log("Jugador gana el envido");
            OnWinEnvido?.Invoke(envidoPoints);
        }
        else
        {
            Debug.Log("Enemigo gana el envido");
        }
        envidoState = EnvidoState.None;
        envidoResolved = true;
    }

    public void ResolveTruco()
    {
        Debug.Log("Truco aceptado");

        switch (trucoState)
        {
            case TrucoState.Truco:
                gameDataSO.trucoPoints = runDataSO.trucoPointsStat;
                break;

            case TrucoState.Retruco:
                gameDataSO.trucoPoints = runDataSO.retrucoPointsStat;
                break;

            case TrucoState.ValeCuatro:
                gameDataSO.trucoPoints = runDataSO.valeCuatroPointsStat;
                break;
        }
        trucoState = TrucoState.None;
    }

    public int CalculateEnvido(List<Card> hand)
    {
        int maxEnvido = 0;

        for (int i = 0; i < hand.Count; i++)
        {
            for (int j = i + 1; j < hand.Count; j++)
            {
                Card c1 = hand[i];
                Card c2 = hand[j];

                if (c1.cardDataSO.suit == c2.cardDataSO.suit)
                {
                    int v1 = GetEnvidoValue(c1.cardDataSO.value);
                    int v2 = GetEnvidoValue(c2.cardDataSO.value);

                    int total = v1 + v2 + 20;

                    if (total > maxEnvido)
                        maxEnvido = total;
                }
            }
        }

        if (maxEnvido == 0)
        {
            foreach (var card in hand)
            {
                int value = GetEnvidoValue(card.cardDataSO.value);

                if (value > maxEnvido)
                    maxEnvido = value;
            }
        }

        return maxEnvido;
    }

    private int GetEnvidoValue(int value)
    {
        if (value >= 10)
            return 0;

        return value;
    }

    public bool IsFirstRound()
    {
        return currentRound == 0;
    }

    void EnemyTurn()
    {
        currentState = GameState.EnemyTurn;

        enemyAI.RespondToPlayer(this);
    }

    public void WaitEnemyResponse()
    {
        currentState = GameState.WaitingEnemyResponse;
        Invoke(nameof(EnemyTurn), 1.5f);
    }

    public void EndPlayerResponse()
    {
        currentState = GameState.PlayerTurn;
    }

    public void EndEnemyTurn()
    {
        currentState = GameState.PlayerTurn;
    }

    public void WaitPlayerResponse()
    {
        currentState = GameState.PlayerTurn;
    }

    public void EndRound()
    {
        Debug.Log("Fin de ronda");
    }
}