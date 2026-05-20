using UnityEngine;

public class ScoreSystem
{
    private float _basePoints;
    private float _baseMult;
    private float _currentPoints;
    private float _currentMult;

    public float CurrentPoints => _currentPoints;
    public float CurrentMult => _currentMult;
    public float TotalScore => _currentPoints * _currentMult;

    public ScoreSystem(float basePoints, float baseMult)
    {
        _basePoints = basePoints;
        _baseMult = baseMult;
        Reset();
    }

    public void Reset()
    {
        _currentPoints = _basePoints;
        _currentMult = _baseMult;
    }

    public void AddPoints(float points) => _currentPoints += points;
    public void AddMult(float mult) => _currentMult += mult;
    public void MultiplyMult(float mult) => _currentMult *= mult;
    public void MultiplyPoints(float points) => _currentPoints *= points;
}