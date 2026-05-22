using System;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public event Action<bool> OnChangePause; // IsPause

    public static PauseManager Instance;

    private bool _isPause;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _isPause = false;
        OnChangePause?.Invoke(_isPause);
    }

    public void ChangePause()
    {
        _isPause = !_isPause;
        OnChangePause?.Invoke(_isPause);
    }
}