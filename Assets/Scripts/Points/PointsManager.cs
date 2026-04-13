using System;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    public static event Action<int> OnPointsCalculated;
    [SerializeField] private RunDataSO _runData;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void CalculatePoints(int points, int multAdd, int multX)
    {
        int calculation = points * (multAdd * multX);
        OnPointsCalculated?.Invoke(calculation);
    }
}
