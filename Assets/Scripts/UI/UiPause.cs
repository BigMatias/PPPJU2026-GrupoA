using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiPause : MonoBehaviour
{
    [SerializeField] private Button _btnPause;
    [SerializeField] private GameObject _panelPause;
    [SerializeField] private Button _btnContinue;
    [SerializeField] private Button _btnMainMenu;
    [SerializeField] private Button _btnExit;

    private void Start()
    {
        _panelPause.SetActive(false);

        _btnPause.onClick.AddListener(PausePressed);
        _btnContinue.onClick.AddListener(ContinuePressed);
        _btnMainMenu.onClick.AddListener(MainMenuPressed);
        _btnExit.onClick.AddListener(ExitPressed);
    }

    private void OnDestroy()
    {
        _btnPause.onClick.RemoveAllListeners();
        _btnContinue.onClick.RemoveAllListeners();
        _btnMainMenu.onClick.RemoveAllListeners();
        _btnExit.onClick.RemoveAllListeners();
    }

    private void PausePressed()
    {
        _panelPause.SetActive(true);
        PauseManager.Instance.ChangePause();
    }

    private void ContinuePressed()
    {
        _panelPause.SetActive(false);
        PauseManager.Instance.ChangePause();
    }

    private void MainMenuPressed() => SceneManager.LoadScene("MainMenu");

    private void ExitPressed() => Application.Quit();
}