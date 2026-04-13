using UnityEngine;
using UnityEngine.UI;

public class UIHUD : MonoBehaviour
{
    [Header("Botones base")]
    [SerializeField] private Button trucoButton;
    [SerializeField] private Button envidoButton;
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

    private void Awake()
    {
        trucoButton.onClick.AddListener(OnTrucoButtonClicked);
        envidoButton.onClick.AddListener(OnEnvidoButtonClicked);
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
        faltaEnvidoButton.onClick.RemoveListener(OnFaltaEnvidoButtonClicked);
        florButton.onClick.RemoveListener(OnFlorButtonClicked);
        mazoButton.onClick.RemoveListener(OnMazoButtonClicked);

        acceptButton.onClick.RemoveListener(OnAcceptButtonClicked);
        denyButton.onClick.RemoveListener(OnDenyButtonClicked);
        retrucoButton.onClick.RemoveListener(OnRetrucoButtonClicked);
        valeCuatroButton.onClick.RemoveListener(OnValeCuatroButtonClicked);
    }

    private void OnTrucoButtonClicked()
    {
        if (gm.trucoPlayedThisRound) return;

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

        Debug.Log("Jugador canta VALE CUATRO");

        gm.trucoState = TrucoState.ValeCuatro;
        gm.WaitEnemyResponse();
    }

    private void OnRetrucoButtonClicked()
    {
        if (gm.currentCall != CallType.Truco) return;
        if (gm.callOwner != CallOwner.Enemy) return; 

        gm.callOwner = CallOwner.Player;
        gm.trucoState = TrucoState.Retruco;

        Debug.Log("Jugador canta RETRUCO");

        gm.WaitEnemyResponse();
    }

    private void OnDenyButtonClicked()
    {
        Debug.Log("Jugador rechaza");

        gm.EndRound();
    }

    private void OnAcceptButtonClicked()
    {
        Debug.Log("Jugador acepta");

        if (gm.currentCall == CallType.Truco)
        {
            gm.ResolveTruco();
        }
        else if (gm.currentCall == CallType.Envido)
        {
            gm.ResolveEnvido();
        }

        gm.ResolveCall(); 
        gm.EndPlayerResponse();
    }

    private void OnMazoButtonClicked()
    {
        Debug.Log("Jugador se va al mazo");

        gm.EndRound();
    }

    private void OnFlorButtonClicked()
    {
        Debug.Log("Jugador canta FLOR (no implementado)");

        gm.WaitEnemyResponse();
    }

    private void OnEnvidoButtonClicked()
    {
        Debug.Log("Jugador canta ENVIDO");

        if (gm.envidoState == EnvidoState.None)
            gm.envidoState = EnvidoState.Envido;
        else
            gm.envidoState = EnvidoState.Envido; // redoblar (simplificado)

        gm.WaitEnemyResponse();
    }

    private void OnFaltaEnvidoButtonClicked()
    {
        if (gm.envidoResolved) return;

        gm.currentCall = CallType.Envido;
        gm.callOwner = CallOwner.Player;
        gm.envidoState = EnvidoState.FaltaEnvido;

        Debug.Log("Jugador canta FALTA ENVIDO");

        gm.WaitEnemyResponse();
    }

    //  ACTUALIZAR HUD
    void UpdateHUD()
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
    }

    void ResetAll()
    {
        trucoButton.gameObject.SetActive(false);
        envidoButton.gameObject.SetActive(false);
        florButton.gameObject.SetActive(false);

        retrucoButton.gameObject.SetActive(false);
        valeCuatroButton.gameObject.SetActive(false);

        responsePanel.gameObject.SetActive(false);
    }

    // TRUCO LOGIC
    void HandleTruco()
    {
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

            case TrucoState.ValeCuatro:
                // ya no hay más escalado
                break;
        }
    }

    // ENVIDO LOGIC
    void HandleEnvido()
    {
        if (gm.trucoState != TrucoState.None && !gm.IsFirstRound())
            return;

        switch (gm.envidoState)
        {
            case EnvidoState.None:
                envidoButton.gameObject.SetActive(true);
                faltaEnvidoButton.gameObject.SetActive(true);
                break;

            case EnvidoState.Envido:
                envidoButton.gameObject.SetActive(true);
                faltaEnvidoButton.gameObject.SetActive(true);
                break;

            case EnvidoState.RealEnvido:
                envidoButton.gameObject.SetActive(true);
                faltaEnvidoButton.gameObject.SetActive(true);
                break;

            case EnvidoState.FaltaEnvido:
                break;
        }
    }
}