using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private RunDataSO runData;
    [SerializeField] private PlayerActions playerActions;
    [SerializeField] private UIConsole uiConsole;
    [SerializeField] private Hand hand;
    [SerializeField] private UIHUD hud;
    [SerializeField] private TrucoManager trucoManager;
    [SerializeField] private EnvidoManager envidoManager;
    [SerializeField] private CardManager cardManager;
    [SerializeField] private ScoreManager scoreManager;

    public GameState CurrentState { get; private set; }
    public CallType CurrentCall { get; private set; } = CallType.None;
    public CallOwner CallOwner { get; private set; } = CallOwner.None;

    public TrucoState TrucoState => trucoManager.TrucoState;
    public bool TrucoPlayedThisRound => trucoManager.TrucoPlayedThisRound;
    public EnvidoState EnvidoState => envidoManager.EnvidoState;
    public bool EnvidoResolved => envidoManager.EnvidoResolved;
    public int CurrentHandPoints => scoreManager.CurrentHandPoints;
    
    private int _handsPlayedThisRun = 0;
    private bool _playerIsDealer = false;
    private GameState _stateBeforeCall = GameState.PlayerTurn;

    public event Action OnRoundEnd;
    public event Action OnNewHand;

    private Coroutine _enemyTurnCoroutine;


    // ── Coroutine helpers ──────────────────────────────────────────

    public void ScheduleEnemyTurn(float delay)
    {
        if (_enemyTurnCoroutine != null)
            StopCoroutine(_enemyTurnCoroutine);
        _enemyTurnCoroutine = StartCoroutine(EnemyTurnCoroutine(delay));
    }

    public void CancelEnemyTurn()
    {
        if (_enemyTurnCoroutine != null)
        {
            StopCoroutine(_enemyTurnCoroutine);
            _enemyTurnCoroutine = null;
        }
    }

    private IEnumerator EnemyTurnCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        _enemyTurnCoroutine = null;
    
        if (CurrentState == GameState.HandOver) yield break; 
    
        SetState(GameState.EnemyTurn);
        enemyAI.RespondToPlayer(this);
    }

    // ── Hand ───────────────────────────────────────────────────────

    public void StartNewHand()
    {
        CancelEnemyTurn();
        RunManager.Instance.UpdateGameEvent(GameEvents.RoundStart);
        CancelEnemyTurn();
        scoreManager.Reset();

        CurrentCall = CallType.None;
        CallOwner = CallOwner.None;

        _playerIsDealer = UnityEngine.Random.value < runData.chanceToBeDealer;
        bool playerIsMano = !_playerIsDealer;

        trucoManager.ResetHand();
        envidoManager.ResetHand();
        cardManager.ResetHand(playerIsMano);

        hand.DealCards();

        SetState(_playerIsDealer ? GameState.PlayerTurn : GameState.EnemyTurn);

        if (CurrentState == GameState.EnemyTurn)
            ScheduleEnemyTurn(1f);

        OnNewHand?.Invoke();
    }

    // ── Player actions ─────────────────────────────────────────────

    public void PlayerSingsTruco() => trucoManager.PlayerSingsTruco();
    public void PlayerSingsRetruco() => trucoManager.PlayerSingsRetruco();
    public void PlayerSingsValeCuatro() => trucoManager.PlayerSingsValeCuatro();
    public void PlayerSingsEnvido(EnvidoState type) => envidoManager.PlayerSingsEnvido(type);

    public void PlayerAccepts()
    {
        if (CurrentState != GameState.PlayerTurn) return;
        if (CurrentCall == CallType.Truco) trucoManager.PlayerAcceptsTruco();
        else if (CurrentCall == CallType.Envido) envidoManager.PlayerAcceptsEnvido();
    }

    public void PlayerDenies()
    {
        if (CurrentState != GameState.PlayerTurn) return;
        if (CurrentCall == CallType.Truco) trucoManager.PlayerDeniesTruco();
        else if (CurrentCall == CallType.Envido) envidoManager.PlayerDeniesEnvido();
    }

    public void PlayerFolds()
    {
        if (CurrentState != GameState.PlayerTurn) return;
        uiConsole.Write("Me voy al mazo..", ConsoleOwner.Player);
        EndRound(false);
    }

    // ── Enemy actions ──────────────────────────────────────────────

    public void EnemySingsTruco() => trucoManager.EnemySingsTruco();
    public void EnemyRaisesTruco() => trucoManager.EnemyRaisesTruco();
    public void EnemySingsEnvido(EnvidoState type) => envidoManager.EnemySingsEnvido(type);

    public void EnemyAccepts()
    {
        if (CurrentCall == CallType.Truco) trucoManager.EnemyAcceptsTruco();
        else if (CurrentCall == CallType.Envido) envidoManager.EnemyAcceptsEnvido();
    }

    public void EnemyDenies()
    {
        if (CurrentCall == CallType.Truco) trucoManager.EnemyDeniesTruco();
        else if (CurrentCall == CallType.Envido) envidoManager.EnemyDeniesEnvido();
    }

    public void EnemyFolds()
    {
        uiConsole.Write("Me voy al mazo...", ConsoleOwner.Enemy);
        EndRound(true);
    }

    public void EnemyPlaysCard(Card card) => cardManager.EnemyPlaysCard(card);

    // ── Round end ──────────────────────────────────────────────────

    public void EndRound(bool playerWon)
    {
        CancelEnemyTurn();
        SetState(GameState.HandOver);

        if (playerWon)
        {
            uiConsole.Write("You won the hand", ConsoleOwner.Player);
            scoreManager.AddRoundWonPoints();
            RunManager.Instance.UpdateGameEvent(GameEvents.RoundEnd);
        }
        else
        {
            uiConsole.Write("The enemy won the hand", ConsoleOwner.Enemy);
        }

        scoreManager.NotifyFinalScore();
        _handsPlayedThisRun++;
        OnRoundEnd?.Invoke();
    }

    // ── Call helpers ───────────────────────────────────────────────

    public void WaitEnemyResponse()
    {
        _stateBeforeCall = GameState.PlayerTurn;
        CancelEnemyTurn();
        SetState(GameState.WaitingEnemyResponse);
        ScheduleEnemyTurn(1.5f);
    }

    public void ResolveCallEnd()
    {
        if (_stateBeforeCall == GameState.EnemyTurn)
        {
            CancelEnemyTurn();
            SetState(GameState.EnemyTurn);
            ScheduleEnemyTurn(1f);
        }
        else
        {
            CancelEnemyTurn();
            cardManager.AllowEnemyToPlay();
            SetState(GameState.PlayerTurn);
        }
    }

    public void SetCurrentCall(CallType call, CallOwner owner)
    {
        CurrentCall = call;
        CallOwner = owner;
    }

    public void SetStateBeforeCall(GameState state) => _stateBeforeCall = state;
    public void SetState(GameState state) => CurrentState = state;
    

    // ── Helpers ────────────────────────────────────────────────────

    public bool IsFirstRound() => cardManager.IsFirstRound();
}