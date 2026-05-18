using System;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    public static event Action<int> OnPointsCalculated;
    [SerializeField] private RunDataSO _runData;

    private float _points = 0;
    private float _multAdd = 0;
    private float _multX = 0;

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

    private void OnWinEnvido_AddToPoints(int envidoValue, EnvidoState state)
    {
        _points += envidoValue;
        switch (state) // PASAR DATOS A UN SO
        {
            case EnvidoState.None:
                return;

            case EnvidoState.Envido:
                _multAdd += 2f;
                break;

            case EnvidoState.RealEnvido:
                _multAdd += 3f;
                break;

            case EnvidoState.EnvidoEnvido:
                _multAdd += 4f;
                break;

            case EnvidoState.FaltaEnvido:
                _multAdd += 15f;
                break;
        }
        CalculatePoints();
    }

    private void OnWinTruco_AddToMult(TrucoState state)
    {
        switch (state)
        {
            case TrucoState.None:
                return;

            case TrucoState.Truco:
                _multX *= 2f;
                break;

            case TrucoState.Retruco:
                _multX *= 3f;
                break;

            case TrucoState.ValeCuatro:
                _multX *= 4f;
                break;
        }
        CalculatePoints();
    }

    private void CalculatePoints()
    {
        float mult = _multAdd * _multX;
        _points *= mult;
        OnPointsCalculated?.Invoke((int)_points);
        _points = 0;
        _multAdd = 0;
        _multX = 0;
    }
}
