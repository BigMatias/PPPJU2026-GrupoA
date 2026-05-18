using System;
using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public event Action OnGameStart;

	public event Action OnPlayerTurn;

	public event Action OnEnemyTurn;

	public event Action OnPlayerWinHand;
	public event Action OnPlayerLoseHand;

	public event Action OnPlayerWinGame;
	public event Action OnPlayerLoseGame;

	public enum GameStates
	{
		NONE = -1,
		Start,
		Shuffle,
		PlayerTurn,
		PlayerSelectTruco,
		PlayerSelectEnvido,
		EnemyTurn,
		EnemyThink,
		PlayerWinHand,
		PlayerLoseHand,
        PlayerWinGame,
		PlayerLoseGame,
	}

	public enum TrucoStates
	{
		NONE = -1,
		Truco,
		Retruco,
		ValeCuatro
	}

	public enum EnvidoStates
	{
		NONE = -1,
		Envido,
		EnvidoEnvido,
		RealEnvido,
		EnvidoRealEnvido,
		EnvidoEnvidoRealEnvido,
		FaltaEnvido
	}

	private GameDefinitionSO _game;
	private bool _isPlayerTurn = false;
	private GameStates _state = GameStates.NONE;

	private IEnumerator _coroutineWaitForNewState;

    private void Start()
    {
        _game = GlobalManager.instance.currentGame;
        if (_game == null)
        {
            Debug.LogError("THERE WAS AN ERROR IN THE COMBAT'S LOAD");
            return;
        }
        Debug.Log("Combate cargado correctamente", gameObject);

        // _enemyTurnManager.SetData(_battle);

        ChangeState(GameStates.Start);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

	private IEnumerator WaitingForNewState(GameStates state)
	{
		yield return new WaitForSeconds(1f);
		ChangeState(state);
	}

    private void ChangeState(GameStates newState)
    {
		_state = newState;

		switch (_state)
		{
			case GameStates.NONE:
				Debug.LogError("THERE IS NO STATE LOADED!!!");
				break;

			case GameStates.Start:
                WaitAndChangeState(GameStates.Shuffle);
                break;

			case GameStates.Shuffle:
				break;

			case GameStates.PlayerTurn:
				break;

			case GameStates.PlayerSelectTruco:
				break;

			case GameStates.PlayerSelectEnvido:
				break;

			case GameStates.EnemyTurn:
				break;

			case GameStates.EnemyThink:
				break;

			case GameStates.PlayerWinHand:
				OnPlayerWinHand?.Invoke();
                WaitAndChangeState(GameStates.Shuffle);
                break;

			case GameStates.PlayerLoseHand:
				OnPlayerLoseHand?.Invoke();
				WaitAndChangeState(GameStates.Shuffle);
                break;

			case GameStates.PlayerWinGame:
				OnPlayerWinGame?.Invoke();
				break;

			case GameStates.PlayerLoseGame:
				OnPlayerLoseGame?.Invoke();
                break;
		}
	}

	private void SetUpGame()
	{
		_isPlayerTurn = true;
        ChangeState(GameStates.PlayerTurn);
    }

	private void ChangeTurn()
	{
		_isPlayerTurn = !_isPlayerTurn;
		if (_isPlayerTurn)
			ChangeState(GameStates.PlayerTurn);
		else
			ChangeState(GameStates.EnemyTurn);
    }

	private void WaitAndChangeState(GameStates state)
	{
		if (_coroutineWaitForNewState != null)
			StopCoroutine(_coroutineWaitForNewState);

		_coroutineWaitForNewState = WaitingForNewState(state);
		StartCoroutine(_coroutineWaitForNewState);
    }
}