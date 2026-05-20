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

    private GameManager _gm;

    private void Awake()
    {
        trucoButton.onClick.AddListener(() => _gm.PlayerSingsTruco());
        envidoButton.onClick.AddListener(OnEnvidoButtonClicked);
        realEnvidoButton.onClick.AddListener(() => _gm.PlayerSingsEnvido(EnvidoState.RealEnvido));
        faltaEnvidoButton.onClick.AddListener(() => _gm.PlayerSingsEnvido(EnvidoState.FaltaEnvido));
        florButton.onClick.AddListener(() => _gm.WaitPlayerResponse());
        mazoButton.onClick.AddListener(() => _gm.PlayerFolds());
        acceptButton.onClick.AddListener(() => _gm.PlayerAccepts());
        denyButton.onClick.AddListener(() => _gm.PlayerDenies());
        retrucoButton.onClick.AddListener(() => _gm.PlayerSingsRetruco());
        valeCuatroButton.onClick.AddListener(() => _gm.PlayerSingsValeCuatro());
    }

    private void Start()
    {
        _gm = GameManager.Instance;
        InvokeRepeating(nameof(UpdateHUD), 0f, 0.2f);
    }

    private void OnDestroy()
    {
        trucoButton.onClick.RemoveAllListeners();
        envidoButton.onClick.RemoveAllListeners();
        realEnvidoButton.onClick.RemoveAllListeners();
        faltaEnvidoButton.onClick.RemoveAllListeners();
        florButton.onClick.RemoveAllListeners();
        mazoButton.onClick.RemoveAllListeners();
        acceptButton.onClick.RemoveAllListeners();
        denyButton.onClick.RemoveAllListeners();
        retrucoButton.onClick.RemoveAllListeners();
        valeCuatroButton.onClick.RemoveAllListeners();
    }

    private void OnEnvidoButtonClicked()
    {
        EnvidoState type = _gm.EnvidoState == EnvidoState.Envido
            ? EnvidoState.EnvidoEnvido
            : EnvidoState.Envido;
        _gm.PlayerSingsEnvido(type);
    }

    private void UpdateHUD()
    {
        if (_gm == null) return;
        Debug.Log($"UpdateHUD - TrucoState: {_gm.TrucoState}, EnvidoState: {_gm.EnvidoState}, CallOwner: {_gm.CallOwner}, State: {_gm.CurrentState}");

        ResetAll();
        mazoButton.gameObject.SetActive(true);

        if (_gm.CurrentState == GameState.PlayerTurn)
        {
            HandleTruco();
            HandleEnvido();
        }

        responsePanel.SetActive(_gm.TrucoState != TrucoState.None || _gm.EnvidoState != EnvidoState.None);
    }

    private void ResetAll()
    {
        trucoButton.gameObject.SetActive(false);
        envidoButton.gameObject.SetActive(false);
        florButton.gameObject.SetActive(false);
        retrucoButton.gameObject.SetActive(false);
        valeCuatroButton.gameObject.SetActive(false);
        responsePanel.SetActive(false);
    }

    private void HandleTruco()
    {
        if (_gm.CurrentCall == CallType.Envido) return;

        switch (_gm.TrucoState)
        {
            case TrucoState.None:
                if (!_gm.TrucoPlayedThisRound)
                    trucoButton.gameObject.SetActive(true);
                break;
            case TrucoState.Truco:
                if (_gm.CallOwner == CallOwner.Enemy)
                    retrucoButton.gameObject.SetActive(true);
                break;
            case TrucoState.Retruco:
                if (_gm.CallOwner == CallOwner.Enemy)
                    valeCuatroButton.gameObject.SetActive(true);
                break;
        }
    }

    private void HandleEnvido()
    {
        if (_gm.EnvidoResolved) return;
        if (_gm.CurrentCall == CallType.Truco && _gm.CallOwner == CallOwner.Player) return;
        if (_gm.TrucoState != TrucoState.None && !_gm.IsFirstRound()) return;

        switch (_gm.EnvidoState)
        {
            case EnvidoState.None:
            case EnvidoState.Envido:
                envidoButton.gameObject.SetActive(true);
                realEnvidoButton.gameObject.SetActive(true);
                faltaEnvidoButton.gameObject.SetActive(true);
                break;
            case EnvidoState.EnvidoEnvido:
                realEnvidoButton.gameObject.SetActive(true);
                faltaEnvidoButton.gameObject.SetActive(true);
                break;
            case EnvidoState.RealEnvido:
                faltaEnvidoButton.gameObject.SetActive(true);
                break;
            case EnvidoState.FaltaEnvido:
                break;
        }
    }
}