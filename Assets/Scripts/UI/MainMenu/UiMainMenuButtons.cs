using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiMainMenuButtons : MonoBehaviour
{
    [SerializeField] private string _gameScene;
    [Header("Buttons")]
    [SerializeField] private Button _btnStart;
    [SerializeField] private Button _btnSettings;
    [SerializeField] private Button _btnCredits;
    [SerializeField] private Button _btnExit;

    private void Start()
    {
        _btnStart.onClick.AddListener(StartPressed);
        _btnSettings.onClick.AddListener(SettingsPressed);
        _btnCredits.onClick.AddListener(CreditsPressed);
        _btnExit.onClick.AddListener(ExitPressed);
    }

    private void OnDestroy()
    {
        _btnStart.onClick.RemoveAllListeners();
        _btnSettings.onClick.RemoveAllListeners();
        _btnCredits.onClick.RemoveAllListeners();
        _btnExit.onClick.RemoveAllListeners();
    }

    private void StartPressed()
    {
        SceneManager.LoadScene(_gameScene);
    }

    private void SettingsPressed()
    {
        return;
    }

    private void CreditsPressed()
    {
        return;
    }

    private void ExitPressed()
    {
        Application.Quit();
    }
}
