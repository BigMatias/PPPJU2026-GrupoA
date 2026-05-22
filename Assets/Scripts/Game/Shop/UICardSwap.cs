using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UICardSwap : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private Image _cardImage;
    [SerializeField] private TMP_Text _cardName;
    [SerializeField] private Button _swapButton;
    [SerializeField] private TMP_Text _swapButtonText;

    private CardSwapManager _swapManager;
    private PlayerActions _playerActions;
    private bool _isSwapModeActive = false;

    private void Awake()
    {
        _swapManager = RunManager.Instance.CardSwapManager;
        _playerActions = RunManager.Instance.GameManager.GetComponent<PlayerActions>();
    }
    private void Start()
    {
        _swapManager.OnPendingCardSet += OnPendingCardSet;
        _swapManager.OnPendingCardCleared += OnPendingCardCleared;

        _swapButton.onClick.AddListener(OnSwapButtonClicked);

        _panel.SetActive(false);

        if (_swapManager.HasPendingCard)
            OnPendingCardSet(_swapManager.PendingCard);
    }
    private void OnDestroy()
    {
        _swapManager.OnPendingCardSet -= OnPendingCardSet;
        _swapManager.OnPendingCardCleared -= OnPendingCardCleared;
        _swapButton.onClick.RemoveAllListeners();
    }
    // When a pending letter is saved, we display the panel
    private void OnPendingCardSet(CardDataSO cardData)
    {
        _cardImage.sprite = cardData.artwork;
        _cardName.text = cardData.name;
        _panel.SetActive(true);
        _isSwapModeActive = false;
        _swapButtonText.text = "Cambiar";
    }
    // When the swap is made, we hide the panel
    private void OnPendingCardCleared()
    {
        _isSwapModeActive = false;
        _playerActions.SetSwapMode(false, null);
        _panel.SetActive(false);
    }
    private void OnSwapButtonClicked()
    {
        _isSwapModeActive = !_isSwapModeActive;

        if (_isSwapModeActive)
        {
            _swapButtonText.text = "Cancelar";
            _playerActions.SetSwapMode(true, _swapManager.PendingCard);
        }
        else
        {
            _swapButtonText.text = "Cambiar";
            _playerActions.SetSwapMode(false, null);
        }
    }
}