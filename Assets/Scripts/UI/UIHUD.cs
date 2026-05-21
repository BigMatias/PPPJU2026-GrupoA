using UnityEngine;
using UnityEngine.UI;

public class UIHUD : MonoBehaviour
{
    [Header("Botones base")]
    [SerializeField] private GameObject _baseSection;
    [SerializeField] private Button _trucoButton;
    [SerializeField] private Button _envidoButton;
    [SerializeField] private Button _mazoButton;

    [Header("Botones truco")]
    [SerializeField] private GameObject _trucoSection;
    [SerializeField] private Button _retrucoButton;
    [SerializeField] private Button _valeCuatroButton;

    [Header("Botones envido")]
    [SerializeField] private GameObject _envidoSection;
    [SerializeField] private Button _envidoSectionEnvidoButton;
    [SerializeField] private Button _realEnvidoButton;
    [SerializeField] private Button _faltaEnvidoButton;
    [SerializeField] private Button _goBackButton;

    [Header("Respuestas")]
    [SerializeField] private GameObject _responsePanel;
    [SerializeField] private Button _acceptButton;
    [SerializeField] private Button _denyButton;

    private GameManager _gm;

    private void Awake()
    {
        _gm = GameManager.Instance;
    }

    private void OnEnable()
    {
        _gm.OnEnemySingTruco += TrucoSangByEnemy_ShowUi;
        _gm.OnEnemySingEnvido += EnvidoSangByEnemy_ShowUi;
    }

    private void Start()
    {
        _trucoButton.onClick.AddListener(() => _gm.PlayerSingsTruco());
        _envidoButton.onClick.AddListener(EnvidoSectionClicked);
        _mazoButton.onClick.AddListener(() => _gm.PlayerFolds());

        _retrucoButton.onClick.AddListener(RetrucoClicked);
        _valeCuatroButton.onClick.AddListener(ValeCuatroClicked);

        _envidoSectionEnvidoButton.onClick.AddListener(EnvidoButtonClicked);
        _realEnvidoButton.onClick.AddListener(RealEnvidoClicked);
        _faltaEnvidoButton.onClick.AddListener(FaltaEnvidoClicked);
        _goBackButton.onClick.AddListener(GoBackClicked);

        _acceptButton.onClick.AddListener(AcceptClicked);
        _denyButton.onClick.AddListener(DenyClicked);

        ResetSections();
    }

    private void OnDisable()
    {
        _gm.OnEnemySingTruco -= TrucoSangByEnemy_ShowUi;
        _gm.OnEnemySingEnvido -= EnvidoSangByEnemy_ShowUi;
    }

    private void OnDestroy()
    {
        _trucoButton.onClick.RemoveAllListeners();
        _envidoButton.onClick.RemoveAllListeners();
        _mazoButton.onClick.RemoveAllListeners();

        _retrucoButton.onClick.RemoveAllListeners();
        _valeCuatroButton.onClick.RemoveAllListeners();

        _envidoSectionEnvidoButton.onClick.RemoveAllListeners();
        _realEnvidoButton.onClick.RemoveAllListeners();
        _faltaEnvidoButton.onClick.RemoveAllListeners();
        _goBackButton.onClick.RemoveAllListeners();

        _acceptButton.onClick.RemoveAllListeners();
        _denyButton.onClick.RemoveAllListeners();
    }

    private void ResetSections()
    {
        _baseSection.SetActive(true);
        _envidoSection.SetActive(false);
        _trucoSection.SetActive(false);
        _responsePanel.SetActive(false);

        Debug.Log($"TrucoState: {_gm.TrucoState}, EnvidoState: {_gm.EnvidoState}, CallOwner: {_gm.CallOwner}, State: {_gm.CurrentState}");
    }

    private void EnvidoSectionClicked()
    {
        _baseSection.SetActive(false);
        _envidoSection.SetActive(true);

        _envidoSectionEnvidoButton.interactable = (_gm.EnvidoState != EnvidoState.EnvidoEnvido);
        _goBackButton.interactable = true;
    }

    private void TrucoSangByEnemy_ShowUi()
    {
        _baseSection.SetActive(false);
        _trucoSection.SetActive(true);
        _responsePanel.SetActive(true);

        _retrucoButton.interactable = (_gm.TrucoState == TrucoState.Truco);
        _valeCuatroButton.interactable = (_gm.TrucoState == TrucoState.Retruco);

        Debug.Log($"UpdateHUD - TrucoState: {_gm.TrucoState}, EnvidoState: {_gm.EnvidoState}, CallOwner: {_gm.CallOwner}, State: {_gm.CurrentState}");
    }

    private void EnvidoSangByEnemy_ShowUi()
    {
        _baseSection.SetActive(false);
        _envidoSection.SetActive(true);
        _responsePanel.SetActive(true);

        _envidoSectionEnvidoButton.interactable = (_gm.EnvidoState == EnvidoState.None || _gm.EnvidoState == EnvidoState.Envido);
        _realEnvidoButton.interactable = (_gm.EnvidoState == EnvidoState.None || _gm.EnvidoState == EnvidoState.Envido || _gm.EnvidoState == EnvidoState.EnvidoEnvido);
        _faltaEnvidoButton.interactable = (_gm.EnvidoState != EnvidoState.FaltaEnvido);
        _goBackButton.interactable = false;

        Debug.Log($"UpdateHUD - TrucoState: {_gm.TrucoState}, EnvidoState: {_gm.EnvidoState}, CallOwner: {_gm.CallOwner}, State: {_gm.CurrentState}");
    }

    private void RetrucoClicked()
    {
        ResetSections();
        _gm.PlayerSingsRetruco();
        Debug.Log($"UpdateHUD - TrucoState: {_gm.TrucoState}, EnvidoState: {_gm.EnvidoState}, CallOwner: {_gm.CallOwner}, State: {_gm.CurrentState}");
    }

    private void ValeCuatroClicked()
    {
        ResetSections();
        _gm.PlayerSingsValeCuatro();
        Debug.Log($"UpdateHUD - TrucoState: {_gm.TrucoState}, EnvidoState: {_gm.EnvidoState}, CallOwner: {_gm.CallOwner}, State: {_gm.CurrentState}");
    }

    private void EnvidoButtonClicked()
    {
        ResetSections();
        EnvidoState type = (_gm.EnvidoState == EnvidoState.Envido)
            ? EnvidoState.EnvidoEnvido
            : EnvidoState.Envido;
        _gm.PlayerSingsEnvido(type);
    }

    private void RealEnvidoClicked()
    {
        ResetSections();
        _gm.PlayerSingsEnvido(EnvidoState.RealEnvido);
    }

        ResetAll();
        mazoButton.gameObject.SetActive(true);

        if (_gm.CurrentState == GameState.PlayerTurn)
        {
            HandleTruco();
            HandleEnvido();
            if (_gm.TrucoState != TrucoState.None || _gm.EnvidoState  != EnvidoState.None)
                responsePanel.SetActive(_gm.TrucoState != TrucoState.None || _gm.EnvidoState != EnvidoState.None);
        }

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
    private void FaltaEnvidoClicked()
    {
        ResetSections();
        _gm.PlayerSingsEnvido(EnvidoState.FaltaEnvido);
    }

    private void GoBackClicked()
    {
        _envidoSection.SetActive(false);
        _baseSection.SetActive(true);
    }

    private void AcceptClicked()
    {
        ResetSections();
        _gm.PlayerAccepts();
    }

    private void DenyClicked()
    {
        ResetSections();
        _gm.PlayerDenies();
    }

    /* //private void UpdateHUD()
    //{
    //    if (_gm == null) return;
    //    Debug.Log($"UpdateHUD - TrucoState: {_gm.TrucoState}, EnvidoState: {_gm.EnvidoState}, CallOwner: {_gm.CallOwner}, State: {_gm.CurrentState}");

    //    ResetAll();
    //    _mazoButton.gameObject.SetActive(true);

    //    if (_gm.CurrentState == GameState.PlayerTurn)
    //    {
    //        HandleTruco();
    //        HandleEnvido();
    //    }

    //    _responsePanel.SetActive(_gm.TrucoState != TrucoState.None || _gm.EnvidoState != EnvidoState.None);
    //}

    //private void ResetAll()
    //{
    //    _trucoButton.gameObject.SetActive(false);
    //    _envidoButton.gameObject.SetActive(false);
    //    _retrucoButton.gameObject.SetActive(false);
    //    _valeCuatroButton.gameObject.SetActive(false);
    //    _responsePanel.SetActive(false);
    //}

    //private void HandleTruco()
    //{
    //    if (_gm.CurrentCall == CallType.Envido) return;

    //    switch (_gm.TrucoState)
    //    {
    //        case TrucoState.None:
    //            if (!_gm.TrucoPlayedThisRound)
    //                _trucoButton.gameObject.SetActive(true);
    //            break;
    //        case TrucoState.Truco:
    //            if (_gm.CallOwner == CallOwner.Enemy)
    //                _retrucoButton.gameObject.SetActive(true);
    //            break;
    //        case TrucoState.Retruco:
    //            if (_gm.CallOwner == CallOwner.Enemy)
    //                _valeCuatroButton.gameObject.SetActive(true);
    //            break;
    //    }
    //}

    //private void HandleEnvido()
    //{
    //    if (_gm.EnvidoResolved) return;
    //    if (_gm.CurrentCall == CallType.Truco && _gm.CallOwner == CallOwner.Player) return;
    //    if (_gm.TrucoState != TrucoState.None && !_gm.IsFirstRound()) return;

    //    switch (_gm.EnvidoState)
    //    {
    //        case EnvidoState.None:
    //            break;

    //        case EnvidoState.Envido:
    //            _envidoButton.gameObject.SetActive(true);
    //            _realEnvidoButton.gameObject.SetActive(true);
    //            _faltaEnvidoButton.gameObject.SetActive(true);
    //            break;

    //        case EnvidoState.EnvidoEnvido:
    //            _realEnvidoButton.gameObject.SetActive(true);
    //            _faltaEnvidoButton.gameObject.SetActive(true);
    //            break;

    //        case EnvidoState.RealEnvido:
    //            _faltaEnvidoButton.gameObject.SetActive(true);
    //            break;

    //        case EnvidoState.FaltaEnvido:
    //            break;
    //    }
    //} */
}