using System;
using UnityEngine;

public class TrucoManager : MonoBehaviour
{
    public event Action OnEnemySingTruco;

    [SerializeField] private GameManager _gm;
    [SerializeField] private RunDataSO _runData;
    [SerializeField] private UIConsole _uiConsole;
    [SerializeField] private ScoreManager _scoreManager;

    public TrucoState TrucoState { get; private set; } = TrucoState.None;
    public bool TrucoPlayedThisRound { get; private set; } = false;

    public void ResetHand()
    {
        TrucoState = TrucoState.None;
        TrucoPlayedThisRound = false;
    }

    // ── Player ─────────────────────────────────────────────────────

    public void PlayerSingsTruco()
    {
        if (TrucoPlayedThisRound || _gm.CurrentState != GameState.PlayerTurn) return;

        TrucoState = TrucoState.Truco;
        _gm.SetCurrentCall(CallType.Truco, CallOwner.Player);
        TrucoPlayedThisRound = true;
        _uiConsole.Write("¡Truco!", ConsoleOwner.Player);

        _gm.WaitEnemyResponse();
    }

    public void PlayerSingsRetruco()
    {
        if (_gm.CurrentCall != CallType.Truco || _gm.CallOwner != CallOwner.Enemy || _gm.CurrentState != GameState.PlayerTurn) return;

        TrucoState = TrucoState.Retruco;
        _gm.SetCurrentCall(CallType.Truco, CallOwner.Player);
        _uiConsole.Write("¡Retruco!", ConsoleOwner.Player);

        _gm.WaitEnemyResponse();
    }

    public void PlayerSingsValeCuatro()
    {
        if (TrucoState != TrucoState.Retruco || _gm.CallOwner != CallOwner.Enemy || _gm.CurrentState != GameState.PlayerTurn) return;

        TrucoState = TrucoState.ValeCuatro;
        _uiConsole.Write("¡Vale Cuatro!", ConsoleOwner.Player);

        _gm.WaitEnemyResponse();
    }

    public void PlayerAcceptsTruco()
    {
        _uiConsole.Write("Quiero", ConsoleOwner.Player);
        AcceptTruco();
        _gm.ResolveCallEnd();
    }

    public void PlayerDeniesTruco()
    {
        _uiConsole.Write("No quiero", ConsoleOwner.Player);
        _gm.EndRound(false);
    }

    // ── Enemy ──────────────────────────────────────────────────────

    public void EnemySingsTruco()
    {
        TrucoState = TrucoState.Truco;
        _gm.SetCurrentCall(CallType.Truco, CallOwner.Enemy);
        _gm.SetStateBeforeCall(GameState.EnemyTurn);
        _gm.CancelEnemyTurn();
        _gm.SetState(GameState.PlayerTurn);
        _uiConsole.Write("¡Truco!", ConsoleOwner.Enemy);
        OnEnemySingTruco?.Invoke();
    }

    public void EnemyRaisesTruco()
    {
        if (TrucoState == TrucoState.Truco)
        {
            _uiConsole.Write("¡Retruco!", ConsoleOwner.Enemy);
            TrucoState = TrucoState.Retruco;
        }
        else if (TrucoState == TrucoState.Retruco)
        {
            _uiConsole.Write("¡Vale Cuatro!", ConsoleOwner.Enemy);
            TrucoState = TrucoState.ValeCuatro;
        }

        _gm.SetCurrentCall(CallType.Truco, CallOwner.Enemy);
        _gm.CancelEnemyTurn();
        _gm.SetState(GameState.PlayerTurn);
        OnEnemySingTruco?.Invoke();
    }

    public void EnemyAcceptsTruco()
    {
        _uiConsole.Write("Quiero", ConsoleOwner.Enemy);
        AcceptTruco();
        _gm.ResolveCallEnd();
    }

    public void EnemyDeniesTruco()
    {
        _uiConsole.Write("No quiero", ConsoleOwner.Enemy);
        _gm.EndRound(true);
    }

    // ── Internal ───────────────────────────────────────────────────

    private void AcceptTruco()
    {
        switch (TrucoState)
        {
            case TrucoState.Truco: _scoreManager.MultiplyMult(_runData.trucoMult); break;
            case TrucoState.Retruco: _scoreManager.MultiplyMult(_runData.retrucoMult); break;
            case TrucoState.ValeCuatro: _scoreManager.MultiplyMult(_runData.valeCuatroMult); break;
        }
        TrucoPlayedThisRound = true;
        TrucoState = TrucoState.None;
        _gm.SetCurrentCall(CallType.None, CallOwner.None);
    }
}