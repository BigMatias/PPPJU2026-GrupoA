using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinAndLoseScreen : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Button _btnMainMenu;
    [SerializeField] private Button _btnExit;

    private void Start()
    {
        _panel.SetActive(false);

        _btnMainMenu.onClick.AddListener(MainMenuClicked);
        _btnExit.onClick.AddListener(ExitClicked);
    }

    private void OnEnable()
    {
        RunManager.Instance.RoundManager.OnWinGame += OnWinGame_ShowUI;
        RunManager.Instance.RoundManager.OnLoseGame += OnLoseGame_ShowUI;
    }

    private void OnDisable()
    {
        RunManager.Instance.RoundManager.OnWinGame -= OnWinGame_ShowUI;
        RunManager.Instance.RoundManager.OnLoseGame -= OnLoseGame_ShowUI;
    }

    private void OnDestroy()
    {
        _panel.SetActive(false);

        _btnMainMenu.onClick.RemoveAllListeners();
        _btnExit.onClick.RemoveAllListeners();
    }

    private void OnWinGame_ShowUI()
    {
        _panel.SetActive(true);
        _text.text = "YOU WIN!!!";
    }

    private void OnLoseGame_ShowUI()
    {
        _panel.SetActive(true);
        _text.text = "YOU LOSE...";
    }

    private void MainMenuClicked() => SceneManager.LoadScene("MainMenu");

    private void ExitClicked() => Application.Quit();
}