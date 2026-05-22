using System;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public event Action<bool> OnChangePause; // IsPause

    private bool _isPause;

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