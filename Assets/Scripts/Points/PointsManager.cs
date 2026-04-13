using System;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    public static event Action<int> OnPointsCalculated;
    [SerializeField] private RunDataSO _runData;

    private int _points = 0;
    private float _mult = 0;

    private void OnEnable()
    {
        GameManager.OnWinEnvido += OnWinEnvido_AddToPoints;
        GameManager.OnWinTruco += OnWinTruco_AddToMult;
    }

    private void OnDisable()
    {
        GameManager.OnWinEnvido -= OnWinEnvido_AddToPoints;
        GameManager.OnWinTruco -= OnWinTruco_AddToMult;
    }

    private void OnWinEnvido_AddToPoints(int envidoValue)
    {
        _points += envidoValue;
    }

    private void OnWinTruco_AddToMult(TrucoState state)
    {
        switch (state)
        {
            case TrucoState.None:
                return;

            case TrucoState.Truco:
                _mult *= 2f;
                break;

            case TrucoState.Retruco:
                _mult *= 3f;
                break;

            case TrucoState.ValeCuatro:
                _mult *= 4f;
                break;
        }
    }

    private void CalculatePoints(int points, int multAdd, int multX)
    {
        _points *= multAdd * multX;
        OnPointsCalculated?.Invoke(_points);
        _points = 0;
    }
}
