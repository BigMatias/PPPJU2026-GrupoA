using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private GameDataSO gameDataSO;
    [SerializeField] private RunDataSO runDataSO;
    [SerializeField] private PlayerActions playerActions;

    public static GameManager Instance;
    public GameState currentState;
    public TrucoState trucoState = TrucoState.None;
    public EnvidoState envidoState = EnvidoState.None;
    public CallType currentCall = CallType.None;
    public CallOwner callOwner = CallOwner.None;

    private Card playerCardPlayed;
    private Card enemyCardPlayed;
    private int roundQuantity = 3;
    public bool trucoPlayedThisRound = false;
    public bool envidoResolved = false;
    private int currentRound = 0;
    private int envidoPoints;
    private bool playerWonHand;
    private List<RoundWon> roundsStateList;

    private void Awake()
    {
        Instance = this;
        PlayerActions.onPlayerCardPlayed += OnPlayerCardPlayed;
        EnemyAI.onEnemyCardPlayed += EnemyAI_onEnemyCardPlayed;
    }

    private void Start()
    {
        roundsStateList = new List<RoundWon>(); 
        gameDataSO.trucoPoints = 0;
        gameDataSO.envidoPoints = 0;
        currentState = GameState.PlayerTurn;
    }

    private void OnDestroy()
    {
        PlayerActions.onPlayerCardPlayed -= OnPlayerCardPlayed;
        EnemyAI.onEnemyCardPlayed -= EnemyAI_onEnemyCardPlayed;
    }

    private void OnPlayerCardPlayed(Card card)
    {
        if (currentState != GameState.PlayerTurn)
            return;
        playerCardPlayed = card;

        AfterCardPlayed();
    }

    private void EnemyAI_onEnemyCardPlayed(Card card)
    {
        enemyCardPlayed = card;

        AfterCardPlayed();
    }

    public void CallDenied()
    {
        if (callOwner == CallOwner.Player)
        {
            if (currentCall == CallType.Envido)
            {
                switch (envidoState)
                {
                    case EnvidoState.Envido:
                        {
                            gameDataSO.totalPoints += 1;
                            break;
                        }
                    case EnvidoState.EnvidoEnvido:
                        {
                            gameDataSO.totalPoints += 2;
                            break;
                        }
                    case EnvidoState.RealEnvido:
                        {
                            gameDataSO.totalPoints += 4;
                            break;
                        }
                    case EnvidoState.FaltaEnvido:
                        {
                            //Se debería guardar el envido anterior y dar esa cantidad de puntos
                            break;
                        }
                }
                gameDataSO.envidoPoints += 1;
                envidoState = EnvidoState.None;
                envidoResolved = true;
            }
            if (currentCall == CallType.Truco)
            {
                switch (trucoState)
                {
                    case TrucoState.Truco:
                        {
                            gameDataSO.totalPoints += 1;
                            break;
                        }
                    case TrucoState.Retruco:
                        {
                            gameDataSO.totalPoints += 2;
                            break;
                        }
                    case TrucoState.ValeCuatro:
                        {
                            gameDataSO.totalPoints += 3;
                            break;
                        }
                }
                trucoPlayedThisRound = true;
            }
        }

        if (currentState == GameState.EnemyTurn && !trucoPlayedThisRound)
        {
            gameDataSO.totalPoints += 1;
        }

        currentCall = CallType.None;
        callOwner = CallOwner.None;

    }

    private int GetCardStrength(Card card)
    {
        int value = card.cardDataSO.value;
        Suit suit = card.cardDataSO.suit;

        if (value == 1 && suit == Suit.Espada) return 14;
        if (value == 1 && suit == Suit.Basto) return 13;
        if (value == 7 && suit == Suit.Espada) return 12;
        if (value == 7 && suit == Suit.Oro) return 11;

        if (value == 3) return 10;
        if (value == 2) return 9;
        if (value == 1) return 8;

        if (value == 12) return 7;
        if (value == 11) return 6;
        if (value == 10) return 5;

        if (value == 7) return 4;
        if (value == 6) return 3;
        if (value == 5) return 2;
        if (value == 4) return 1;

        return 0;
    }

    private void AfterCardPlayed()
    {
        if (playerCardPlayed != null && enemyCardPlayed != null)
        {
            int playerStrength = GetCardStrength(playerCardPlayed);
            int enemyStrength = GetCardStrength(enemyCardPlayed);

            Debug.Log($"Player: {playerStrength} vs Enemy: {enemyStrength}");

            if (playerStrength > enemyStrength)
            {
                Debug.Log("Jugador gana la ronda");
                roundsStateList.Add(RoundWon.Player);
                currentState = GameState.PlayerTurn;
            }
            else if (enemyStrength > playerStrength)
            {
                Debug.Log("Enemigo gana la ronda");
                roundsStateList.Add(RoundWon.Enemy);
                currentState = GameState.EnemyTurn;
            }
            else
            {
                Debug.Log("Parda (empate)");
                roundsStateList.Add(RoundWon.Tie);
            }

            playerCardPlayed = null;
            enemyCardPlayed = null;

            currentRound++;

            CheckHandWinner();
        }
        else if (playerCardPlayed != null && enemyCardPlayed == null)
        {
            currentState = GameState.WaitingEnemyResponse;
            Invoke(nameof(EnemyTurn), 1f);
        }
        else if (playerCardPlayed == null && enemyCardPlayed != null)
        {
            EndEnemyTurn();
        }
    }

    private void CheckHandWinner()
    {
        if (roundsStateList.Count > 0)
        {
            RoundWon last = roundsStateList[roundsStateList.Count - 1];

            if (last != RoundWon.Tie)
            {
                for (int i = roundsStateList.Count - 2; i >= 0; i--)
                {
                    if (roundsStateList[i] == RoundWon.Tie)
                    {
                        playerWonHand = (last == RoundWon.Player);
                        EndRound(playerWonHand);
                        return;
                    }

                    if (roundsStateList[i] != last)
                        break;
                }

                if (roundsStateList.Count >= 2 &&
                    roundsStateList[roundsStateList.Count - 2] == last)
                {
                    playerWonHand = (last == RoundWon.Player);
                    EndRound(playerWonHand);
                    return;
                }
            }
        }

        if (currentRound >= roundQuantity)
        {
            foreach (var round in roundsStateList)
            {
                if (round == RoundWon.Player)
                {
                    EndRound(true);
                    return;
                }
                else if (round == RoundWon.Enemy)
                {
                    EndRound(false);
                    return;
                }
            }

            EndRound(true);
            return;
        }

        currentState = GameState.PlayerTurn;
    }

    private void EnemyTurn()
    {
        currentState = GameState.EnemyTurn;

        enemyAI.RespondToPlayer(this);
    }

    public void EnvidoManager(bool envidoSettled)
    {
        if (!envidoSettled)
        {
            switch (envidoState)
            {
                case EnvidoState.Envido:
                    {
                        envidoPoints += runDataSO.envidoPointsStat;
                        break;
                    }
                case EnvidoState.EnvidoEnvido:
                    {
                        envidoPoints += runDataSO.envidoEnvidoPointsStat;
                        break;
                    }
                case EnvidoState.RealEnvido:
                    {
                        envidoPoints += runDataSO.realEnvidoPointsStat;
                        break;
                    }
                case EnvidoState.FaltaEnvido:
                    {
                        envidoPoints += (gameDataSO.pointsNeededToWinRound - gameDataSO.totalPoints);
                        break;
                    }
            }
        }
        else
        {
            Debug.Log("Resolviendo ENVIDO");

            int playerPoints = CalculateEnvido(playerActions.playerHand);
            int enemyPoints = CalculateEnvido(enemyAI.enemyHand);

            Debug.Log("Jugador: " + playerPoints);
            Debug.Log("Enemigo: " + enemyPoints);

            if (playerPoints > enemyPoints)
            {
                Debug.Log("Jugador gana el envido");
                gameDataSO.envidoPoints = envidoPoints;
            }
            else
            {
                Debug.Log("Enemigo gana el envido");
            }
            envidoState = EnvidoState.None;
            envidoResolved = true;
        }
    }

    public void ResolveTruco()
    {
        Debug.Log("Truco aceptado");

        switch (trucoState)
        {
            case TrucoState.Truco:
                gameDataSO.trucoPoints = runDataSO.trucoPointsStat;
                break;

            case TrucoState.Retruco:
                gameDataSO.trucoPoints = runDataSO.retrucoPointsStat;
                break;

            case TrucoState.ValeCuatro:
                gameDataSO.trucoPoints = runDataSO.valeCuatroPointsStat;
                break;
        }
        trucoState = TrucoState.None;
        trucoPlayedThisRound = true;
    }

    private int CalculateEnvido(List<Card> hand)
    {
        int maxEnvido = 0;

        for (int i = 0; i < hand.Count; i++)
        {
            for (int j = i + 1; j < hand.Count; j++)
            {
                Card c1 = hand[i];
                Card c2 = hand[j];

                if (c1.cardDataSO.suit == c2.cardDataSO.suit)
                {
                    int v1 = GetEnvidoValue(c1.cardDataSO.value);
                    int v2 = GetEnvidoValue(c2.cardDataSO.value);

                    int total = v1 + v2 + 20;

                    if (total > maxEnvido)
                        maxEnvido = total;
                }
            }
        }

        if (maxEnvido == 0)
        {
            foreach (var card in hand)
            {
                int value = GetEnvidoValue(card.cardDataSO.value);

                if (value > maxEnvido)
                    maxEnvido = value;
            }
        }

        return maxEnvido;
    }

    private int GetEnvidoValue(int value)
    {
        if (value >= 10)
            return 0;

        return value;
    }

    public bool IsFirstRound()
    {
        return currentRound == 0;
    }

    public void WaitEnemyResponse()
    {
        currentState = GameState.WaitingEnemyResponse;
        Invoke(nameof(EnemyTurn), 1.5f);
    }

    public void EndPlayerResponse(CallOwner previousOwner)
    {
        if (previousOwner == CallOwner.Enemy)
        {
            currentState = GameState.EnemyTurn;
            Invoke(nameof(EnemyTurn), 1f);
        }
        else
        {
            currentState = GameState.PlayerTurn;
        }
    }

    public void EndEnemyTurn()
    {
        currentState = GameState.PlayerTurn;
    }

    public void WaitPlayerResponse()
    {
        currentState = GameState.PlayerTurn;
    }

    public void EndRound(bool playerWonHand)
    {
        Debug.Log("Fin de ronda");

        gameDataSO.totalPoints += gameDataSO.trucoPoints;
        gameDataSO.totalPoints += gameDataSO.envidoPoints;

        Debug.Log("Jugador suma " + gameDataSO.totalPoints + " puntos.");

        gameDataSO.trucoPoints = 0;
        gameDataSO.envidoPoints = 0;
        gameDataSO.totalPoints = 0;
        currentRound = 0;

        trucoPlayedThisRound = false;
        envidoResolved = false;

        trucoState = TrucoState.None;
        envidoState = EnvidoState.None;

        // repartir de nuevo
    }
}