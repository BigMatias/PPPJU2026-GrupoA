using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private PlayerActions playerActions;

    public static GameManager Instance;
    public TrucoState trucoState = TrucoState.None;
    public EnvidoState envidoState = EnvidoState.None;
    public GameState currentState;
    public CallType currentCall = CallType.None;
    public CallOwner callOwner = CallOwner.None;

    public bool trucoPlayedThisRound = false;
    public bool envidoResolved = false;
    public int currentRound = 0; // 0,1,2
    public int trucoPoints = 1;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
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
        Debug.Log("Resolviendo canto...");

        currentCall = CallType.None;
        callOwner = CallOwner.None;

        envidoResolved = true;
    }

    public void ResolveEnvido()
    {
        Debug.Log("Resolviendo ENVIDO");

        int playerPoints = CalculateEnvido(playerActions.playerHand);
        int enemyPoints = CalculateEnvido(enemyAI.enemyHand);

        Debug.Log("Jugador: " + playerPoints);
        Debug.Log("Enemigo: " + enemyPoints);

        if (playerPoints > enemyPoints)
        {
            Debug.Log("Jugador gana el envido");
            // sumar puntos jugador
        }
        else
        {
            Debug.Log("Enemigo gana el envido");
            // sumar puntos enemigo
        }

        envidoResolved = true;
    }

    public void ResolveTruco()
    {
        Debug.Log("Truco aceptado");

        switch (trucoState)
        {
            case TrucoState.Truco:
                trucoPoints = 2;
                break;

            case TrucoState.Retruco:
                trucoPoints = 3;
                break;

            case TrucoState.ValeCuatro:
                trucoPoints = 4;
                break;
        }
    }

    public int CalculateEnvido(List<Card> hand)
    {
        int max = 0;

        foreach (var card in hand)
        {
            int value = Mathf.Min(card.cardDataSO.value, 7); // figuras valen 0
            if (value > max)
                max = value;
        }

        return max;
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