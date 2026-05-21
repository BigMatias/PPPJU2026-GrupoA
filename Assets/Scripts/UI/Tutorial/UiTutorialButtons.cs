using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiTutorialButtons : MonoBehaviour
{
    [SerializeField] private Button _btnGoToMainMenu;
    [SerializeField] private Button _btnExitGame;

    private void Start()
    {
        _btnGoToMainMenu.onClick.AddListener(MainMenuClicked);
        _btnExitGame.onClick.AddListener(ExitGameClicked);
    }

    private void OnDestroy()
    {
        _btnGoToMainMenu.onClick.RemoveAllListeners();
        _btnExitGame.onClick.RemoveAllListeners();
    }

    private void MainMenuClicked() => SceneManager.LoadScene("MainMenu");

    private void ExitGameClicked() => Application.Quit();
}