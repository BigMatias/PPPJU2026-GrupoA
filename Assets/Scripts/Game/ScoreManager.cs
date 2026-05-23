using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public event Action<float, float> OnScoreChanged;         // points, mult
    public event Action<float, float, float> OnCalculateScore; // points, mult, totalScore

    [SerializeField] private RunDataSO _runData;

    private float _totalAccumulatedScore;
    private float _basePoints;
    private float _baseMult;
    private float _currentPoints;
    private float _currentMult;
    public float TotalAccumulatedScore => _totalAccumulatedScore;
    public float CurrentPoints => _currentPoints;
    public float CurrentMult => _currentMult;
    public float TotalScore => _currentPoints * _currentMult;
    public int CurrentHandPoints => (int)TotalScore;

    private void Awake()
    {
        _basePoints = _runData.basePoints;
        _baseMult = _runData.baseMult;
        Reset();
    }

    // ── Core ───────────────────────────────────────────────────────

    public void Reset()
    {
        _currentPoints = _basePoints;
        _currentMult = _baseMult;
        OnScoreChanged?.Invoke(_currentPoints, _currentMult);
    }

    public void AddPoints(float points)
    {
        _currentPoints += points;
        OnScoreChanged?.Invoke(_currentPoints, _currentMult);
    }

    public void MultiplyMult(float mult)
    {
        _currentMult *= mult;
        OnScoreChanged?.Invoke(_currentPoints, _currentMult);
    }

    public void NotifyFinalScore()
    {
        _totalAccumulatedScore += TotalScore;
        OnCalculateScore?.Invoke(_currentPoints, _currentMult, _totalAccumulatedScore);
    }

    // ── Card ───────────────────────────────────────────────────────

    public void AddCardPoints(Card card)
    {
        float points = GetCardStrength(card) * _runData.cardPointMultiplier;
        AddPoints(points);
    }

    public int GetCardStrength(Card card)
    {
        return card.cardDataSO.strength;
    }

    // ── Truco ──────────────────────────────────────────────────────

    public void ApplyTrucoMult(TrucoState state)
    {
        float mult = state switch
        {
            TrucoState.Truco => _runData.trucoMult,
            TrucoState.Retruco => _runData.retrucoMult,
            TrucoState.ValeCuatro => _runData.valeCuatroMult,
            _ => 1f
        };
        MultiplyMult(mult);
    }

    // ── Envido ─────────────────────────────────────────────────────

    public void AddEnvidoPoints(EnvidoState state)
    {
        AddPoints(GetEnvidoPoints(state));
    }

    public void AddEnvidoDeniedPoints(EnvidoState state, CallOwner callOwner)
    {
        if (callOwner == CallOwner.Player)
            AddPoints(GetEnvidoDeniedPoints(state));
    }


    public void AddRoundWonPoints()
    {
        _currentPoints += _runData.pointsOnRoundWon;
        OnScoreChanged?.Invoke(_currentPoints, _currentMult);
    }

    public float GetEnvidoPoints(EnvidoState state) => state switch
    {
        EnvidoState.Envido => _runData.envidoPoints,
        EnvidoState.EnvidoEnvido => _runData.envidoEnvidoPoints,
        EnvidoState.RealEnvido => _runData.realEnvidoPoints,
        EnvidoState.FaltaEnvido => _runData.faltaEnvidoPoints,
        _ => 0f
    };

    public float GetEnvidoDeniedPoints(EnvidoState state) => state switch
    {
        EnvidoState.Envido => 1f,
        EnvidoState.EnvidoEnvido => 2f,
        EnvidoState.RealEnvido => 4f,
        _ => 0f
    };
}