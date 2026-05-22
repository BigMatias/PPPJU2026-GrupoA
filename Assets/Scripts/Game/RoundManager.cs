using System;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private RunDataSO _runData;
    public event Action OnMesaWon;
    public event Action OnMesaLost;
    public event Action OnChicoWon;
    public event Action<int, int, int> OnInfoUpdated; // mesa, chico, manosRestantes
    public event Action<float> OnSetNeededScore;
    // Mesa
    private int _currentMesaPoints;
    private int _manosPlayedThisMesa;
    private int _manosPerMesa;

    // Chico
    private int _currentChico;
    private int _currentMesa;
    private int _mesasWonThisChico;

    // Run
    private int _totalManosPlayed;
    private int _totalChicosPlayed;

    private void Awake()
    {
        _gameManager.OnRoundEnd += OnManoEnd;
    }

    private void OnDestroy()
    {
        _gameManager.OnRoundEnd -= OnManoEnd;
    }

    private void Start()
    {
        _currentChico = 0;
        _currentMesa = 0;
        _totalManosPlayed = 0;
        _totalChicosPlayed = 0;
        StartChico();
    }

    // ── Chico ──────────────────────────────────────────────────────

    private void StartChico()
    {
        _mesasWonThisChico = 0;
        _currentMesa = 0;
        StartMesa();
    }

    private void EndChico(bool playerWon)
    {
        _totalChicosPlayed++;

        if (playerWon)
        {
            _currentChico++;
            _currentMesa = 0;
            _mesasWonThisChico = 0;
            OnChicoWon?.Invoke();
            // abrir shop, después llamar StartChico()
        }
        else
        {
            OnMesaLost?.Invoke();
            // pantalla de derrota
        }
    }

    // ── Mesa ───────────────────────────────────────────────────────

    private void StartMesa()
    {
        _currentMesaPoints = 0;
        _manosPlayedThisMesa = 0;
        _manosPerMesa = _runData.handsPerRound;
        OnSetNeededScore?.Invoke(GetPointsNeededForCurrentMesa());
        OnInfoUpdated?.Invoke(_currentMesa + 1, _currentChico + 1, _manosPerMesa - _manosPlayedThisMesa);
        _gameManager.StartNewHand();
    }

    private void OnManoEnd()
    {
        _manosPlayedThisMesa++;
        _totalManosPlayed++;
        _currentMesaPoints = _gameManager.CurrentHandPoints;
        OnInfoUpdated?.Invoke(_currentMesa + 1, _currentChico + 1, _manosPerMesa - _manosPlayedThisMesa);

        if (CheckMesaWon())
        {
            _mesasWonThisChico++;
            _currentMesa++;
            OnMesaWon?.Invoke();

            if (CheckChicoWon())
                EndChico(true);
            else
                StartMesa();
        }
        else if (CheckMesaLost())
        {
            EndChico(false);
        }
        else
        {
            _gameManager.StartNewHand();
        }
    }

    // ── Checks ─────────────────────────────────────────────────────

    private bool CheckMesaWon() =>
        _currentMesaPoints >= GetPointsNeededForCurrentMesa();

    private bool CheckMesaLost() =>
        _manosPlayedThisMesa >= _manosPerMesa &&
        _currentMesaPoints < GetPointsNeededForCurrentMesa();

    private bool CheckChicoWon()
    {
        if (_currentChico >= _runData.chicos.Length) return true;
        return _currentMesa >= _runData.chicos[_currentChico].pointsPerMesa.Length;
    }

    // ── RunData helpers ────────────────────────────────────────────

    private int GetPointsNeededForCurrentMesa()
    {
        if (_currentChico >= _runData.chicos.Length)
        {
            var lastChico = _runData.chicos[_runData.chicos.Length - 1];
            return lastChico.pointsPerMesa[Mathf.Min(_currentMesa, lastChico.pointsPerMesa.Length - 1)];
        }

        var chico = _runData.chicos[_currentChico];

        if (_currentMesa >= chico.pointsPerMesa.Length)
            return chico.pointsPerMesa[chico.pointsPerMesa.Length - 1];

        return chico.pointsPerMesa[_currentMesa];
    }

    // ── Getters ────────────────────────────────────────────────────

    public int CurrentChico => _currentChico;
    public int CurrentMesa => _currentMesa;
    public int MesasWonThisChico => _mesasWonThisChico;
    public int ManosPlayedThisMesa => _manosPlayedThisMesa;
    public int CurrentMesaPoints => _currentMesaPoints;
    public int ManosPerMesa => _manosPerMesa;

    public void SetManosPerMesa(int value) => _manosPerMesa = value;
}