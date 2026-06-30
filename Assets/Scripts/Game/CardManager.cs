using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] private GameManager _gm;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private RunDataSO _runData;
    [SerializeField] private PlayerActions _playerActions;
    [SerializeField] private EnemyAI _enemyAI;

    private Card _playerCardPlayed;
    private Card _enemyCardPlayed;
    private List<RoundWon> _roundResults = new List<RoundWon>();
    private int _currentRound = 0;
    private bool _playerPlayedThisRound = false;
    private bool _enemyPlayedThisRound = false;
    private bool _playerIsMano = false;

    public int CurrentRound => _currentRound;

    private void Awake()
    {
        _playerActions.OnCardPlayed += OnPlayerCardPlayed;
        _enemyAI.OnCardPlayed += OnEnemyCardPlayed;
    }

    private void OnDestroy()
    {
        _playerActions.OnCardPlayed -= OnPlayerCardPlayed;
        _enemyAI.OnCardPlayed -= OnEnemyCardPlayed;
    }

    public void ResetHand(bool playerIsMano)
    {
        if (_enemyCardPlayed != null) _enemyCardPlayed.cardGO = null;
        if (_playerCardPlayed != null) _playerCardPlayed.cardGO = null;

        _roundResults.Clear();
        _currentRound = 0;
        _playerCardPlayed = null;
        _enemyCardPlayed = null;
        _playerPlayedThisRound = false;
        _enemyPlayedThisRound = false;
        _playerIsMano = playerIsMano;
    }

    public bool IsFirstRound() => _currentRound == 0;

    // ── Card played ────────────────────────────────────────────────

    private void OnPlayerCardPlayed(Card card)
    {
        if (_gm.CurrentState != GameState.PlayerTurn) return;
        if (_playerPlayedThisRound) return;

        _scoreManager.AddCardPoints(card);
        _playerCardPlayed = card;
        _playerPlayedThisRound = true;

        RunManager.Instance.Gauchos.SetContext(
            playedCard: card,
            hand: _playerActions.playerHand
        );

        RunManager.Instance.UpdateGameEvent(GameEvents.CardPlayed);
        AfterCardPlayed();
    }

    private void OnEnemyCardPlayed(Card card)
    {
        EnemyPlaysCard(card);
    }

    public void EnemyPlaysCard(Card card)
    {
        if (_gm.CurrentState == GameState.HandOver) return;
        if (_enemyPlayedThisRound) return;
        if (card.cardGO == null)
        {
            Debug.LogError($"cardGO is null for card: {card.cardDataSO.name}");
            return;
        }
        _enemyCardPlayed = card;
        _enemyPlayedThisRound = true;
        AfterCardPlayed();
    }

    public void AllowEnemyToPlay()
    {
        if (_playerPlayedThisRound)
            _enemyPlayedThisRound = false;
    }

    // ── Resolution ─────────────────────────────────────────────────

    private void AfterCardPlayed()
    {
        if (_playerCardPlayed == null || _enemyCardPlayed == null)
        {
            if (_playerCardPlayed != null)
            {
                _gm.SetState(GameState.WaitingEnemyResponse);
                _gm.ScheduleEnemyTurn(1f);
            }
            else if (_enemyCardPlayed != null)
            {
                _gm.CancelEnemyTurn();
                _gm.SetState(GameState.PlayerTurn);
            }
            return;
        }

        int playerStrength = GetCardStrength(_playerCardPlayed);
        int enemyStrength = GetCardStrength(_enemyCardPlayed);

        RoundWon result;
        Card winningCard = null;

        if (playerStrength > enemyStrength)
        {
            result = RoundWon.Player;
            winningCard = _playerCardPlayed;
        }
        else if (enemyStrength > playerStrength)
        {
            result = RoundWon.Enemy;
            winningCard = _enemyCardPlayed;
        }
        else
            result = RoundWon.Tie;

        if (winningCard != null && winningCard.cardGO != null)
        {
            CardView view = winningCard.cardGO.GetComponent<CardView>();
            view.SetCardToWinner();
            winningCard.cardGO.GetComponent<SpriteRenderer>().sortingOrder += 1;
        }

        _roundResults.Add(result);

        _playerCardPlayed = null;
        _enemyCardPlayed = null;
        _playerPlayedThisRound = false;
        _enemyPlayedThisRound = false;
        _currentRound++;

        CheckHandWinner(result);
    }

    private void CheckHandWinner(RoundWon lastRoundResult)
    {
        if (_roundResults.Count > 0)
        {
            RoundWon last = _roundResults[_roundResults.Count - 1];

            if (last != RoundWon.Tie)
            {
                for (int i = _roundResults.Count - 2; i >= 0; i--)
                {
                    if (_roundResults[i] == RoundWon.Tie)
                    {
                        _gm.EndRound(last == RoundWon.Player);
                        return;
                    }
                    if (_roundResults[i] != last) break;
                }

                if (_roundResults.Count >= 2 && _roundResults[_roundResults.Count - 2] == last)
                {
                    _gm.EndRound(last == RoundWon.Player);
                    return;
                }
            }
        }

        if (_currentRound >= 3)
        {
            foreach (var round in _roundResults)
            {
                if (round == RoundWon.Player) { _gm.EndRound(true); return; }
                if (round == RoundWon.Enemy) { _gm.EndRound(false); return; }
            }
            _gm.EndRound(_playerIsMano);
            return;
        }

        if (lastRoundResult == RoundWon.Enemy)
        {
            _gm.SetState(GameState.EnemyTurn);
            _gm.ScheduleEnemyTurn(1f);
        }
        else if (lastRoundResult == RoundWon.Tie)
        {
            if (_playerIsMano)
                _gm.SetState(GameState.PlayerTurn);
            else
            {
                _gm.SetState(GameState.EnemyTurn);
                _gm.ScheduleEnemyTurn(1f);
            }
        }
        else
        {
            _gm.SetState(GameState.PlayerTurn);
        }
    }

    // ── Score helpers ──────────────────────────────────────────────

    private int GetCardStrength(Card card) => card.cardDataSO.strength;
}