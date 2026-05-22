using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action OnEnemySingTruco;
    public event Action OnEnemySingEnvido;
    public event Action OnRoundEnd;

    public event Action<int, int, int> OnSetRoundInfo; // round, ante, money
    public event Action<float> OnSetNeededScore; // neededScore
    public event Action<float, float, float> OnCalculateScore; // points, mult, totalScore

    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private RunDataSO runData;
    [SerializeField] private PlayerActions playerActions;
    [SerializeField] private UIConsole uiConsole;
    [SerializeField] private Hand hand;
    [SerializeField] private UIHUD hud;

    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }
    public TrucoState TrucoState { get; private set; } = TrucoState.None;
    public EnvidoState EnvidoState { get; private set; } = EnvidoState.None;
    public CallType CurrentCall { get; private set; } = CallType.None;
    public CallOwner CallOwner { get; private set; } = CallOwner.None;
    public bool TrucoPlayedThisRound { get; private set; } = false;
    public bool EnvidoResolved { get; private set; } = false;

    private ScoreSystem _scoreSystem;
    private Card _playerCardPlayed;
    private Card _enemyCardPlayed;
    private List<RoundWon> _roundResults = new List<RoundWon>();
    private int _currentRound = 0;
    private int _ante = 0;
    private int _handsPlayedThisRun = 0;
    private bool _playerIsDealer = false;
    private bool _playerIsMano = false;
    private GameState _stateBeforeCall = GameState.PlayerTurn;
    private bool _enemyPlayedThisRound = false;
    private bool _playerPlayedThisRound = false;

    private Coroutine _enemyTurnCoroutine;

    private void Awake()
    {
        Instance = this;
        playerActions.Initialize(this);
        playerActions.OnCardPlayed += OnPlayerCardPlayed;
        enemyAI.OnCardPlayed += OnEnemyCardPlayed;
    }

    private void Start()
    {
        _scoreSystem = new ScoreSystem(runData.basePoints, runData.baseMult);
        StartHand();
    }

    private void OnDestroy()
    {
        playerActions.OnCardPlayed -= OnPlayerCardPlayed;
        enemyAI.OnCardPlayed -= OnEnemyCardPlayed;
    }

    // ── Coroutine helpers ──────────────────────────────────────────

    private void ScheduleEnemyTurn(float delay)
    {
        if (_enemyTurnCoroutine != null)
            StopCoroutine(_enemyTurnCoroutine);
        _enemyTurnCoroutine = StartCoroutine(EnemyTurnCoroutine(delay));
    }

    private void CancelEnemyTurn()
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
        SetState(GameState.EnemyTurn);
        enemyAI.RespondToPlayer(this);
    }

    // ── Hand ───────────────────────────────────────────────────────

    private void StartHand()
    {
        RunManager.Instance.UpdateGameEvent(GameEvents.RoundStart);
        OnSetNeededScore?.Invoke(runData.pointsNeededToWin);
        CancelEnemyTurn();
        _scoreSystem.Reset();
        _roundResults.Clear();
        _currentRound = 0;
        TrucoPlayedThisRound = false;
        EnvidoResolved = false;
        TrucoState = TrucoState.None;
        EnvidoState = EnvidoState.None;
        CurrentCall = CallType.None;
        CallOwner = CallOwner.None;

        _playerIsDealer = UnityEngine.Random.value < runData.chanceToBeDealer;
        _playerIsMano = !_playerIsDealer;

        hand.DealCards();

        _enemyPlayedThisRound = false;
        _playerPlayedThisRound = false;

        SetState(_playerIsDealer ? GameState.PlayerTurn : GameState.EnemyTurn);

        if (CurrentState == GameState.EnemyTurn)
            ScheduleEnemyTurn(1f);
    }

    // ── Player actions ─────────────────────────────────────────────

    public void PlayerSingsTruco()
    {
        if (TrucoPlayedThisRound || CurrentState != GameState.PlayerTurn) return;

        TrucoState = TrucoState.Truco;
        CurrentCall = CallType.Truco;
        CallOwner = CallOwner.Player;
        TrucoPlayedThisRound = true;
        uiConsole.Write("¡Truco!", ConsoleOwner.Player);

        WaitEnemyResponse();
    }

    public void PlayerSingsRetruco()
    {
        if (CurrentCall != CallType.Truco || CallOwner != CallOwner.Enemy || CurrentState != GameState.PlayerTurn) return;

        TrucoState = TrucoState.Retruco;
        CallOwner = CallOwner.Player;
        uiConsole.Write("¡Retruco!", ConsoleOwner.Player);

        WaitEnemyResponse();
    }

    public void PlayerSingsValeCuatro()
    {
        if (TrucoState != TrucoState.Retruco || CallOwner != CallOwner.Enemy || CurrentState != GameState.PlayerTurn) return;

        TrucoState = TrucoState.ValeCuatro;
        uiConsole.Write("¡Vale Cuatro!", ConsoleOwner.Player);

        WaitEnemyResponse();
    }

    public void PlayerSingsEnvido(EnvidoState type)
    {
        if (CurrentState != GameState.PlayerTurn || EnvidoResolved || !IsFirstRound()) return;

        bool valid = (EnvidoState, type) switch
        {
            (EnvidoState.None, _) => true,
            (EnvidoState.Envido, EnvidoState.Envido) => true,
            (EnvidoState.Envido, EnvidoState.RealEnvido) => true,
            (EnvidoState.Envido, EnvidoState.FaltaEnvido) => true,
            (EnvidoState.EnvidoEnvido, EnvidoState.RealEnvido) => true,
            (EnvidoState.EnvidoEnvido, EnvidoState.FaltaEnvido) => true,
            (EnvidoState.RealEnvido, EnvidoState.FaltaEnvido) => true,
            _ => false
        };

        if (!valid) return;

        string message = type switch
        {
            EnvidoState.Envido => EnvidoState == EnvidoState.Envido ? "¡Envido Envido!" : "¡Envido!",
            EnvidoState.EnvidoEnvido => "¡Envido Envido!",
            EnvidoState.RealEnvido => "¡Real Envido!",
            EnvidoState.FaltaEnvido => "¡Falta Envido!",
            _ => ""
        };

        uiConsole.Write(message, ConsoleOwner.Player);
        EnvidoState = type;
        CurrentCall = CallType.Envido;
        CallOwner = CallOwner.Player;
        _scoreSystem.AddPoints(GetEnvidoPoints());
        WaitEnemyResponse();
    }

    public void PlayerAccepts()
    {
        if (CurrentState != GameState.PlayerTurn) return;

        uiConsole.Write("Quiero", ConsoleOwner.Player);
        if (CurrentCall == CallType.Truco)
        {
            AcceptTruco();
            ResolveCallEnd();
        }
        else if (CurrentCall == CallType.Envido)
        {
            AcceptEnvido();
            ResolveCallEnd();
        }

    }

    public void PlayerDenies()
    {
        if (CurrentState != GameState.PlayerTurn) return;

        uiConsole.Write("No quiero", ConsoleOwner.Player);
        if (CurrentCall == CallType.Truco)
        {
            EndRound(false);
            return;
        }
        if (CurrentCall == CallType.Envido)
        {
            DenyEnvido();
            ResolveCallEnd();
        }

    }

    public void PlayerFolds()
    {
        if (CurrentState != GameState.PlayerTurn) return;
        uiConsole.Write("Me voy al mazo..", ConsoleOwner.Player);
        EndRound(false);
    }

    // ── Enemy actions ──────────────────────────────────────────────

    public void EnemySingsTruco()
    {
        TrucoState = TrucoState.Truco;
        CurrentCall = CallType.Truco;
        CallOwner = CallOwner.Enemy;
        _stateBeforeCall = GameState.EnemyTurn;
        CancelEnemyTurn();
        SetState(GameState.PlayerTurn);
        uiConsole.Write("¡Truco!", ConsoleOwner.Enemy);
        OnEnemySingTruco?.Invoke();
    }

    public void EnemyRaisesTruco()
    {
        if (TrucoState == TrucoState.Truco)
        {
            uiConsole.Write("¡Retruco!", ConsoleOwner.Enemy);
            TrucoState = TrucoState.Retruco;
        }
        else if (TrucoState == TrucoState.Retruco)
        {
            uiConsole.Write("¡Vale Cuatro!", ConsoleOwner.Enemy);
            TrucoState = TrucoState.ValeCuatro;
        }

        CallOwner = CallOwner.Enemy;
        CancelEnemyTurn();
        SetState(GameState.PlayerTurn);
        OnEnemySingTruco?.Invoke();
    }

    public void EnemySingsEnvido(EnvidoState type)
    {
        _stateBeforeCall = GameState.EnemyTurn;

        bool valid = (EnvidoState, type) switch
        {
            (EnvidoState.None, _) => true,
            (EnvidoState.Envido, EnvidoState.Envido) => true,
            (EnvidoState.Envido, EnvidoState.RealEnvido) => true,
            (EnvidoState.Envido, EnvidoState.FaltaEnvido) => true,
            (EnvidoState.EnvidoEnvido, EnvidoState.RealEnvido) => true,
            (EnvidoState.EnvidoEnvido, EnvidoState.FaltaEnvido) => true,
            (EnvidoState.RealEnvido, EnvidoState.FaltaEnvido) => true,
            _ => false
        };

        if (!valid) return;

        string message = type switch
        {
            EnvidoState.Envido => EnvidoState == EnvidoState.Envido ? "¡Envido Envido!" : "¡Envido!",
            EnvidoState.EnvidoEnvido => "¡Envido Envido!",
            EnvidoState.RealEnvido => "¡Real Envido!",
            EnvidoState.FaltaEnvido => "¡Falta Envido!",
            _ => ""
        };

        uiConsole.Write(message, ConsoleOwner.Enemy);
        EnvidoState = type;
        CurrentCall = CallType.Envido;
        CallOwner = CallOwner.Enemy;
        CancelEnemyTurn();
        SetState(GameState.PlayerTurn);
        OnEnemySingEnvido?.Invoke();
    }

    public void EnemyAccepts()
    {
        uiConsole.Write("Quiero", ConsoleOwner.Enemy);
        if (CurrentCall == CallType.Truco)
        {
            AcceptTruco();
            ResolveCallEnd();
        }
        else if (CurrentCall == CallType.Envido)
        {
            AcceptEnvido();
            ResolveCallEnd();
        }
    }

    public void EnemyDenies()
    {
        uiConsole.Write("No quiero", ConsoleOwner.Enemy);
        if (CurrentCall == CallType.Truco)
        {
            EndRound(true);
            return;
        }
        if (CurrentCall == CallType.Envido)
        {
            DenyEnvido();
            ResolveCallEnd();
        }
    }

    public void EnemyFolds()
    {
        uiConsole.Write("Me voy al mazo...", ConsoleOwner.Enemy);
        EndRound(true);
    }

    public void EnemyPlaysCard(Card card)
    {
        if (_enemyPlayedThisRound) return;
        _enemyCardPlayed = card;
        _enemyPlayedThisRound = true;
        AfterCardPlayed();
    }

    // ── Card resolution ────────────────────────────────────────────

    private void OnPlayerCardPlayed(Card card)
    {
        if (CurrentState != GameState.PlayerTurn) return;
        if (_playerPlayedThisRound) return;
        _scoreSystem.AddPoints(GetCardPoints(card));
        _playerCardPlayed = card;
        _playerPlayedThisRound = true;
        AfterCardPlayed();
        RunManager.Instance.UpdateGameEvent(GameEvents.CardPlayed);
    }

    private void OnEnemyCardPlayed(Card card)
    {
        EnemyPlaysCard(card);
    }

    private void AfterCardPlayed()
    {
        if (_playerCardPlayed == null || _enemyCardPlayed == null)
        {
            if (_playerCardPlayed != null)
            {
                SetState(GameState.WaitingEnemyResponse);
                ScheduleEnemyTurn(1f);
            }
            else if (_enemyCardPlayed != null)
            {
                CancelEnemyTurn();
                SetState(GameState.PlayerTurn);
            }
            return;
        }

        int playerStrength = GetCardStrength(_playerCardPlayed);
        int enemyStrength = GetCardStrength(_enemyCardPlayed);

        RoundWon result;
        if (playerStrength > enemyStrength)
            result = RoundWon.Player;
        else if (enemyStrength > playerStrength)
            result = RoundWon.Enemy;
        else
            result = RoundWon.Tie;

        _roundResults.Add(result);
        _playerCardPlayed = null;
        _enemyCardPlayed = null;
        _playerPlayedThisRound = false;
        _enemyPlayedThisRound = false;
        _currentRound++;
        CheckHandWinner(result);
    }

    private void CheckHandWinner(RoundWon lastRoundResult)
    {
        if (_roundResults.Count > 0)
        {
            RoundWon last = _roundResults[_roundResults.Count - 1];

            if (last != RoundWon.Tie)
            {
                for (int i = _roundResults.Count - 2; i >= 0; i--)
                {
                    if (_roundResults[i] == RoundWon.Tie)
                    {
                        EndRound(last == RoundWon.Player);
                        return;
                    }
                    if (_roundResults[i] != last) break;
                }

                if (_roundResults.Count >= 2 && _roundResults[_roundResults.Count - 2] == last)
                {
                    EndRound(last == RoundWon.Player);
                    return;
                }
            }
        }

        if (_currentRound >= 3)
        {
            foreach (var round in _roundResults)
            {
                if (round == RoundWon.Player) { EndRound(true); return; }
                if (round == RoundWon.Enemy) { EndRound(false); return; }
            }
            EndRound(_playerIsMano);
            return;
        }

        if (lastRoundResult == RoundWon.Enemy)
        {
            SetState(GameState.EnemyTurn);
            ScheduleEnemyTurn(1f);
        }
        else if (lastRoundResult == RoundWon.Tie)
        {
            if (_playerIsMano)
                SetState(GameState.PlayerTurn);
            else
            {
                SetState(GameState.EnemyTurn);
                ScheduleEnemyTurn(1f);
            }
        }
        else
        {
            SetState(GameState.PlayerTurn);
        }
    }

    public void EndRound(bool playerWon)
    {
        CancelEnemyTurn();
        SetState(GameState.HandOver);

        if (playerWon)
        {
            uiConsole.Write("You won the hand", ConsoleOwner.Player);
            _scoreSystem.AddPoints(15f);
            runData.points += (int)_scoreSystem.TotalScore;
            RunManager.Instance.UpdateGameEvent(GameEvents.RoundEnd);
        }
        else
        {
            uiConsole.Write("The enemy won the hand", ConsoleOwner.Player);
        }

        _handsPlayedThisRun++;
        OnCalculateScore?.Invoke(_scoreSystem.CurrentPoints, _scoreSystem.CurrentMult, _scoreSystem.TotalScore);
        OnRoundEnd?.Invoke();

        if (CheckWin())
            EndAnte(true);
        else if (CheckLose())
            EndAnte(false);
        else
            StartHand();
    }

    private bool CheckWin()
    {
        if (runData.points >= runData.pointsNeededToWin)
            return true;

        return false;
    }

    private bool CheckLose()
    {
        if (_handsPlayedThisRun >= runData.handsPerRound && runData.points <= runData.pointsNeededToWin)
            return true;

        return false;
    }

    private void EndAnte(bool playerWon)
    {
        if (playerWon)
        {
            if (_handsPlayedThisRun >= 10)
            {
                // ui win screen
            }
            else
            {
                RunManager.Instance.MoneySystem.AddMoneyForWinningRound(0); // aca hay q pasarle cuantas rounds sobraron, pero no se cual es la variable para hacer la cuenta
                runData.pointsNeededToWin += 100;
                _ante++;
                OnSetRoundInfo?.Invoke(_currentRound, _ante, RunManager.Instance.MoneySystem.CurrentMoney); // change 0 for money
                // open shop
                StartHand(); // esto deberia llamarse despues, desde el shop, esta aca para probar q ande nomas
            }
        }
        else
        {
            // ui lose screen
        }
    }

    // ── Truco / Envido helpers ─────────────────────────────────────

    private void AcceptTruco()
    {
        switch (TrucoState)
        {
            case TrucoState.Truco: _scoreSystem.MultiplyMult(runData.trucoMult); break;
            case TrucoState.Retruco: _scoreSystem.MultiplyMult(runData.retrucoMult); break;
            case TrucoState.ValeCuatro: _scoreSystem.MultiplyMult(runData.valeCuatroMult); break;
        }
        TrucoPlayedThisRound = true;
        CurrentCall = CallType.None;
        CallOwner = CallOwner.None;
        TrucoState = TrucoState.None;
    }

    private void AcceptEnvido()
    {
        int playerPoints = CalculateEnvido(playerActions.playerHand);
        int enemyPoints = CalculateEnvido(enemyAI.enemyHand);

        uiConsole.Write($"Your points: {playerPoints} ", ConsoleOwner.Player);
        uiConsole.Write($"Enemy points: {enemyPoints} ", ConsoleOwner.Enemy);

        if (playerPoints >= enemyPoints)
        {
            uiConsole.Write("You win!", ConsoleOwner.Player);
            _scoreSystem.AddPoints(GetEnvidoPoints());
        }
        else
        {
            uiConsole.Write("Enemy wins!", ConsoleOwner.Enemy);
        }

        EnvidoState = EnvidoState.None;
        EnvidoResolved = true;
        CurrentCall = CallType.None;
        CallOwner = CallOwner.None;
    }

    private void DenyEnvido()
    {
        if (CallOwner == CallOwner.Player)
            _scoreSystem.AddPoints(GetEnvidoDeniedPoints());

        EnvidoState = EnvidoState.None;
        EnvidoResolved = true;
        CurrentCall = CallType.None;
        CallOwner = CallOwner.None;
    }

    private void ResolveCallEnd()
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
            SetState(GameState.PlayerTurn);
        }
    }

    // ── Enemy trigger ──────────────────────────────────────────────

    private void WaitEnemyResponse()
    {
        _stateBeforeCall = GameState.PlayerTurn;
        CancelEnemyTurn();
        SetState(GameState.WaitingEnemyResponse);
        ScheduleEnemyTurn(1.5f);
    }

    // ── Score helpers ──────────────────────────────────────────────

    private float GetCardPoints(Card card) => GetCardStrength(card) * runData.cardPointMultiplier;

    private float GetEnvidoPoints() => EnvidoState switch
    {
        EnvidoState.Envido => runData.envidoPoints,
        EnvidoState.EnvidoEnvido => runData.envidoEnvidoPoints,
        EnvidoState.RealEnvido => runData.realEnvidoPoints,
        EnvidoState.FaltaEnvido => runData.faltaEnvidoPoints,
        _ => 0f
    };

    private float GetEnvidoDeniedPoints() => EnvidoState switch
    {
        EnvidoState.Envido => 1f,
        EnvidoState.EnvidoEnvido => 2f,
        EnvidoState.RealEnvido => 4f,
        _ => 0f
    };

    private int GetCardStrength(Card card) => card.cardDataSO.strength;

    private int CalculateEnvido(List<Card> hand)
    {
        int maxEnvido = 0;

        for (int i = 0; i < hand.Count; i++)
            for (int j = i + 1; j < hand.Count; j++)
                if (hand[i].cardDataSO.suit == hand[j].cardDataSO.suit)
                {
                    int total = GetEnvidoValue(hand[i].cardDataSO.value) +
                                GetEnvidoValue(hand[j].cardDataSO.value) + 20;
                    if (total > maxEnvido) maxEnvido = total;
                }

        if (maxEnvido == 0)
            foreach (var card in hand)
            {
                int v = GetEnvidoValue(card.cardDataSO.value);
                if (v > maxEnvido) maxEnvido = v;
            }

        return maxEnvido;
    }

    private int GetEnvidoValue(int value) => value >= 10 ? 0 : value;

    private void SetState(GameState state) => CurrentState = state;

    public bool IsFirstRound() => _currentRound == 0;
}