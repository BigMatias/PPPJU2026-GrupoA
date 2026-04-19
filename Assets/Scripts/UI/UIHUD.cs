using System;
using UnityEngine;
using UnityEngine.UI;

public class UIHUD : MonoBehaviour
{
    [Header("Botones base")]
    [SerializeField] private Button trucoButton;
    [SerializeField] private Button envidoButton;
    [SerializeField] private Button realEnvidoButton;
    [SerializeField] private Button florButton;
    [SerializeField] private Button mazoButton;

    [Header("Respuestas")]
    [SerializeField] private GameObject responsePanel;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button denyButton;

    [Header("Reemplazos")]
    [SerializeField] private Button faltaEnvidoButton;
    [SerializeField] private Button retrucoButton;
    [SerializeField] private Button valeCuatroButton;

    private GameManager gm;
    private bool envidoSettled;

    private void Awake()
    {
        trucoButton.onClick.AddListener(OnTrucoButtonClicked);
        envidoButton.onClick.AddListener(OnEnvidoButtonClicked);
        realEnvidoButton.onClick.AddListener(OnRealEnvidoButtonClicked);
        faltaEnvidoButton.onClick.AddListener(OnFaltaEnvidoButtonClicked);
        florButton.onClick.AddListener(OnFlorButtonClicked);
        mazoButton.onClick.AddListener(OnMazoButtonClicked);

        acceptButton.onClick.AddListener(OnAcceptButtonClicked);
        denyButton.onClick.AddListener(OnDenyButtonClicked);
        retrucoButton.onClick.AddListener(OnRetrucoButtonClicked);
        valeCuatroButton.onClick.AddListener(OnValeCuatroButtonClicked);
    }

    private void Start()
    {
        gm = GameManager.Instance;
        UpdateHUD();
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(UpdateHUD), 0f, 0.2f);
    }

    private void OnDestroy()
    {
        trucoButton.onClick.RemoveListener(OnTrucoButtonClicked);
        envidoButton.onClick.RemoveListener(OnEnvidoButtonClicked);
        realEnvidoButton.onClick.RemoveListener(OnRealEnvidoButtonClicked);
        faltaEnvidoButton.onClick.RemoveListener(OnFaltaEnvidoButtonClicked);
        florButton.onClick.RemoveListener(OnFlorButtonClicked);
        mazoButton.onClick.RemoveListener(OnMazoButtonClicked);

        acceptButton.onClick.RemoveListener(OnAcceptButtonClicked);
        denyButton.onClick.RemoveListener(OnDenyButtonClicked);
        retrucoButton.onClick.RemoveListener(OnRetrucoButtonClicked);
        valeCuatroButton.onClick.RemoveListener(OnValeCuatroButtonClicked);
    }

    // ------ Truco buttons -------
    private void OnTrucoButtonClicked()
    {
        if (gm.trucoPlayedThisRound) return;
        if (gm.currentState != GameState.PlayerTurn) return;

        gm.trucoPlayedThisRound = true;

        gm.currentCall = CallType.Truco;
        gm.callOwner = CallOwner.Player;
        gm.trucoState = TrucoState.Truco;

        Debug.Log("Jugador canta TRUCO");

        gm.WaitEnemyResponse();
    }

    private void OnValeCuatroButtonClicked()
    {
        if (gm.trucoState != TrucoState.Retruco) return;
        if (gm.callOwner != CallOwner.Enemy) return;
        if (gm.currentState != GameState.PlayerTurn) return;

        Debug.Log("Jugador canta VALE CUATRO");

        gm.trucoState = TrucoState.ValeCuatro;
        gm.WaitEnemyResponse();
    }

    private void OnRetrucoButtonClicked()
    {
        if (gm.currentCall != CallType.Truco) return;
        if (gm.callOwner != CallOwner.Enemy) return;
        if (gm.currentState != GameState.PlayerTurn) return;

        gm.callOwner = CallOwner.Player;
        gm.trucoState = TrucoState.Retruco;

        Debug.Log("Jugador canta RETRUCO");

        gm.WaitEnemyResponse();
    }
    // ------ Aceptar / rechazar buttons -------
    private void OnDenyButtonClicked()
    {
        if (gm.currentState != GameState.PlayerTurn) return;
        CallOwner previousOwner = gm.callOwner;
        if (gm.currentCall == CallType.Truco)
        {
            Debug.Log("Jugador rechaza el truco");
            bool playerWonHand = false;
            gm.EndRound(playerWonHand);
        }
        else if (gm.currentCall == CallType.Envido)
        {
            Debug.Log("Jugador rechaza el envido");
            gm.CallDenied();
            gm.EndPlayerResponse(previousOwner);
        }
    }

    private void OnAcceptButtonClicked()
    {
        if (gm.currentState != GameState.PlayerTurn) return;
        Debug.Log("Jugador acepta");

        CallOwner previousOwner = gm.callOwner;

        if (gm.currentCall == CallType.Truco)
        {
            gm.ResolveTruco();
        }
        else if (gm.currentCall == CallType.Envido)
        {
            envidoSettled = true;
            gm.EnvidoManager(envidoSettled);
        }

        gm.CallDenied();

        gm.EndPlayerResponse(previousOwner); 
    }

    private void OnMazoButtonClicked()
    {
        if (gm.currentState != GameState.PlayerTurn) return;
        Debug.Log("Jugador se va al mazo");
        bool playerWonHand = false;
        gm.EndRound(playerWonHand);
    }

    // ------ Envido buttons ------
    private void OnRealEnvidoButtonClicked()
    {
        if (gm.currentState != GameState.PlayerTurn) return;
        Debug.Log("Jugador canta Real ENVIDO");

        gm.envidoState = EnvidoState.RealEnvido;
        envidoSettled = false;
        gm.EnvidoManager(envidoSettled);
        gm.WaitEnemyResponse();
    }

    private void OnFlorButtonClicked()
    {
        if (gm.currentState != GameState.PlayerTurn) return;
        Debug.Log("Jugador canta FLOR (no implementado)");

        gm.WaitEnemyResponse();
    }

    private void OnEnvidoButtonClicked()
    {
        if (gm.currentState != GameState.PlayerTurn) return;
        Debug.Log("Jugador canta ENVIDO");

        if (gm.envidoState == EnvidoState.None)
            gm.envidoState = EnvidoState.Envido;
        else if (gm.envidoState == EnvidoState.Envido)
            gm.envidoState = EnvidoState.EnvidoEnvido;

        envidoSettled = false;
        gm.EnvidoManager(envidoSettled);
        gm.WaitEnemyResponse();
    }

    private void OnFaltaEnvidoButtonClicked()
    {
        if (gm.currentState != GameState.PlayerTurn) return;
        if (gm.envidoResolved) return;

        gm.callOwner = CallOwner.Player;
        gm.envidoState = EnvidoState.FaltaEnvido;

        envidoSettled = false;
        gm.EnvidoManager(envidoSettled);
        Debug.Log("Jugador canta FALTA ENVIDO");

        gm.WaitEnemyResponse();
    }

    //  ACTUALIZAR HUD
    private void UpdateHUD()
    {
        ResetAll();

        mazoButton.gameObject.SetActive(true);

        if (gm.currentState == GameState.PlayerTurn)
        {
            HandleTruco();
            HandleEnvido();
        }

        if (gm.trucoState != TrucoState.None || gm.envidoState != EnvidoState.None)
        {
            responsePanel.SetActive(true);
        }
        else
        {
            responsePanel.SetActive(false);
        }
    }

    private void ResetAll()
    {
        trucoButton.gameObject.SetActive(false);
        envidoButton.gameObject.SetActive(false);
        florButton.gameObject.SetActive(false);

        retrucoButton.gameObject.SetActive(false);
        valeCuatroButton.gameObject.SetActive(false);

        responsePanel.gameObject.SetActive(false);
    }

    // TRUCO LOGIC
    private void HandleTruco()
    {
        if (gm.trucoPlayedThisRound)
            return;

        switch (gm.trucoState)
        {
            case TrucoState.None:
                trucoButton.gameObject.SetActive(true);
                break;

            case TrucoState.Truco:
                retrucoButton.gameObject.SetActive(true);
                break;

            case TrucoState.Retruco:
                valeCuatroButton.gameObject.SetActive(true);
                break;
        }
    }

    // ENVIDO LOGIC
    private void HandleEnvido()
    {
        if (gm.envidoResolved)
            return;

        if (gm.trucoState != TrucoState.None && !gm.IsFirstRound())
            return;

        switch (gm.envidoState)
        {
            case EnvidoState.None:
                envidoButton.gameObject.SetActive(true);
                realEnvidoButton.gameObject.SetActive(true);
                faltaEnvidoButton.gameObject.SetActive(true);
                break;

            case EnvidoState.Envido:
                envidoButton.gameObject.SetActive(true);
                realEnvidoButton.gameObject.SetActive(true);
                faltaEnvidoButton.gameObject.SetActive(true);
                break;

            case EnvidoState.EnvidoEnvido:
                envidoButton.gameObject.SetActive(false);
                realEnvidoButton.gameObject.SetActive(true);
                faltaEnvidoButton.gameObject.SetActive(true);
                break;

            case EnvidoState.RealEnvido:
                envidoButton.gameObject.SetActive(true);
                faltaEnvidoButton.gameObject.SetActive(true);
                break;

            case EnvidoState.FaltaEnvido:
                envidoButton.gameObject.SetActive(false);
                realEnvidoButton.gameObject.SetActive(false);
                faltaEnvidoButton.gameObject.SetActive(false);
                break;
        }
    }
}