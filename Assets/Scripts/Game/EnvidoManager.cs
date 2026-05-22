using System;
using System.Collections.Generic;
using UnityEngine;

public class EnvidoManager : MonoBehaviour
{
    public event Action OnEnemySingEnvido;

    [SerializeField] private GameManager _gm;
    [SerializeField] private RunDataSO _runData;
    [SerializeField] private UIConsole _uiConsole;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private PlayerActions _playerActions;
    [SerializeField] private EnemyAI _enemyAI;

    public EnvidoState EnvidoState { get; private set; } = EnvidoState.None;
    public bool EnvidoResolved { get; private set; } = false;

    public void ResetHand()
    {
        EnvidoState = EnvidoState.None;
        EnvidoResolved = false;
    }

    // ── Validation ─────────────────────────────────────────────────

    private bool IsValidRaise(EnvidoState current, EnvidoState next)
    {
        return (current, next) switch
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
    }

    private string GetMessage(EnvidoState current, EnvidoState type)
    {
        return type switch
        {
            EnvidoState.Envido => current == EnvidoState.Envido ? "¡Envido Envido!" : "¡Envido!",
            EnvidoState.EnvidoEnvido => "¡Envido Envido!",
            EnvidoState.RealEnvido => "¡Real Envido!",
            EnvidoState.FaltaEnvido => "¡Falta Envido!",
            _ => ""
        };
    }

    // ── Player ─────────────────────────────────────────────────────

    public void PlayerSingsEnvido(EnvidoState type)
    {
        if (_gm.CurrentState != GameState.PlayerTurn || EnvidoResolved || !_gm.IsFirstRound()) return;
        if (!IsValidRaise(EnvidoState, type)) return;

        _uiConsole.Write(GetMessage(EnvidoState, type), ConsoleOwner.Player);
        EnvidoState = type;
        _gm.SetCurrentCall(CallType.Envido, CallOwner.Player);
        _scoreManager.AddPoints(GetEnvidoPoints());
        _gm.WaitEnemyResponse();
    }

    public void PlayerAcceptsEnvido()
    {
        _uiConsole.Write("Quiero", ConsoleOwner.Player);
        AcceptEnvido();
        _gm.ResolveCallEnd();
    }

    public void PlayerDeniesEnvido()
    {
        _uiConsole.Write("No quiero", ConsoleOwner.Player);
        DenyEnvido();
        _gm.ResolveCallEnd();
    }

    // ── Enemy ──────────────────────────────────────────────────────

    public void EnemySingsEnvido(EnvidoState type)
    {
        if (!IsValidRaise(EnvidoState, type)) return;

        _gm.SetStateBeforeCall(GameState.EnemyTurn);
        _uiConsole.Write(GetMessage(EnvidoState, type), ConsoleOwner.Enemy);
        EnvidoState = type;
        _gm.SetCurrentCall(CallType.Envido, CallOwner.Enemy);
        _gm.CancelEnemyTurn();
        _gm.SetState(GameState.PlayerTurn);
        OnEnemySingEnvido?.Invoke();
    }

    public void EnemyAcceptsEnvido()
    {
        _uiConsole.Write("Quiero", ConsoleOwner.Enemy);
        AcceptEnvido();
        _gm.ResolveCallEnd();
    }

    public void EnemyDeniesEnvido()
    {
        _uiConsole.Write("No quiero", ConsoleOwner.Enemy);
        DenyEnvido();
        _gm.ResolveCallEnd();
    }

    // ── Internal ───────────────────────────────────────────────────

    private void AcceptEnvido()
    {
        int playerPoints = CalculateEnvido(_playerActions.playerHand);
        int enemyPoints = CalculateEnvido(_enemyAI.enemyHand);

        _uiConsole.Write($"Your points: {playerPoints}", ConsoleOwner.Player);
        _uiConsole.Write($"Enemy points: {enemyPoints}", ConsoleOwner.Enemy);

        if (playerPoints >= enemyPoints)
        {
            _uiConsole.Write("You win!", ConsoleOwner.Player);
            _scoreManager.AddPoints(GetEnvidoPoints());
        }
        else
        {
            _uiConsole.Write("Enemy wins!", ConsoleOwner.Enemy);
        }

        ResolveEnvido();
    }

    private void DenyEnvido()
    {
        if (_gm.CallOwner == CallOwner.Player)
            _scoreManager.AddPoints(GetEnvidoDeniedPoints());

        ResolveEnvido();
    }

    private void ResolveEnvido()
    {
        EnvidoState = EnvidoState.None;
        EnvidoResolved = true;
        _gm.SetCurrentCall(CallType.None, CallOwner.None);
    }

    // ── Score helpers ──────────────────────────────────────────────

    private float GetEnvidoPoints() => EnvidoState switch
    {
        EnvidoState.Envido => _runData.envidoPoints,
        EnvidoState.EnvidoEnvido => _runData.envidoEnvidoPoints,
        EnvidoState.RealEnvido => _runData.realEnvidoPoints,
        EnvidoState.FaltaEnvido => _runData.faltaEnvidoPoints,
        _ => 0f
    };

    private float GetEnvidoDeniedPoints() => EnvidoState switch
    {
        EnvidoState.Envido => 1f,
        EnvidoState.EnvidoEnvido => 2f,
        EnvidoState.RealEnvido => 4f,
        _ => 0f
    };

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
}