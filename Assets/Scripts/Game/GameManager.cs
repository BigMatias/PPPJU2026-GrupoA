using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action OnEnemySingTruco;
    public event Action OnEnemySingEnvido;
    public event Action OnRoundEnd;

    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private RunDataSO runData;
    [SerializeField] private PlayerActions playerActions;
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
    private int _handsPlayedThisRun = 0;
    private bool _playerIsDealer = false;
    private bool _playerIsMano = false;
    private GameState _stateBeforeCall = GameState.PlayerTurn;

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

        WaitEnemyResponse();
    }

    public void PlayerSingsRetruco()
    {
        if (CurrentCall != CallType.Truco || CallOwner != CallOwner.Enemy || CurrentState != GameState.PlayerTurn) return;

        TrucoState = TrucoState.Retruco;
        CallOwner = CallOwner.Player;

        WaitEnemyResponse();
    }

    public void PlayerSingsValeCuatro()
    {
        if (TrucoState != TrucoState.Retruco || CallOwner != CallOwner.Enemy || CurrentState != GameState.PlayerTurn) return;

        TrucoState = TrucoState.ValeCuatro;

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

        EnvidoState = type;
        CurrentCall = CallType.Envido;
        CallOwner = CallOwner.Player;

        _scoreSystem.AddPoints(GetEnvidoPoints());

        WaitEnemyResponse();
    }

    public void PlayerAccepts()
    {
        if (CurrentState != GameState.PlayerTurn) return;

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
        OnEnemySingTruco?.Invoke();
    }

    public void EnemyRaisesTruco()
    {
        if (TrucoState == TrucoState.Truco)
            TrucoState = TrucoState.Retruco;
        else if (TrucoState == TrucoState.Retruco)
            TrucoState = TrucoState.ValeCuatro;

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

        EnvidoState = type;
        CurrentCall = CallType.Envido;
        CallOwner = CallOwner.Enemy;
        CancelEnemyTurn();
        SetState(GameState.PlayerTurn);
        OnEnemySingEnvido?.Invoke();
    }

    public void EnemyAccepts()
    {
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

    public void EnemyFolds() => EndRound(true);

    public void EnemyPlaysCard(Card card)
    {
        _enemyCardPlayed = card;
        AfterCardPlayed();
    }

    // ── Card resolution ────────────────────────────────────────────

    private void OnPlayerCardPlayed(Card card)
    {
        if (CurrentState != GameState.PlayerTurn) return;
        _scoreSystem.AddPoints(GetCardPoints(card));
        _playerCardPlayed = card;
        AfterCardPlayed();
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
        hud.gameObject.SetActive(false);

        if (playerWon)
        {
            _scoreSystem.AddPoints(10f);
            runData.points += (int)_scoreSystem.TotalScore;
        }

        _handsPlayedThisRun++;

        if (runData.points >= runData.pointsNeededToWin || _handsPlayedThisRun >= runData.handsPerRound)
            return;

        OnRoundEnd?.Invoke();
        StartHand();
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
    }

    private void AcceptEnvido()
    {
        int playerPoints = CalculateEnvido(playerActions.playerHand);
        int enemyPoints = CalculateEnvido(enemyAI.enemyHand);

        if (playerPoints >= enemyPoints)
            _scoreSystem.AddPoints(GetEnvidoPoints());

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

    public void WaitPlayerResponse()
    {
        CancelEnemyTurn();
        SetState(GameState.PlayerTurn);
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

    private int GetCardStrength(Card card)
    {
        int value = card.cardDataSO.value;
        Suit suit = card.cardDataSO.suit;

        if (value == 1 && suit == Suit.Espada) return 14;
        if (value == 1 && suit == Suit.Basto) return 13;
        if (value == 7 && suit == Suit.Espada) return 12;
        if (value == 7 && suit == Suit.Oro) return 11;
        if (value == 3) return 10;
        if (value == 2) return 9;
        if (value == 1) return 8;
        if (value == 12) return 7;
        if (value == 11) return 6;
        if (value == 10) return 5;
        if (value == 7) return 4;
        if (value == 6) return 3;
        if (value == 5) return 2;
        if (value == 4) return 1;
        return 0;
    }

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