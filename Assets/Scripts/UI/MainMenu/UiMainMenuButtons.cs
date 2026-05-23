using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiMainMenuButtons : MonoBehaviour
{
    [Header("Canvas references")]
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private GameObject creditsCanvas;

    [Header("Buttons reference")]
    [SerializeField] private Button playBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private Button tutorialBtn;
    [SerializeField] private Button creditsBtn;
    [SerializeField] private Button quitBtn;
    [SerializeField] private Button settingsBtnBack;
    [SerializeField] private Button creditsBtnBack;

    [Header("Sounds")]
    [SerializeField] private AudioClip buttonClickSfx;
    [SerializeField] private AudioClip buttonHoverSfx;
    private void Awake()
    {
        playBtn.onClick.AddListener(PlayGame);
        settingsBtn.onClick.AddListener(OpenSettings);
        tutorialBtn.onClick.AddListener(GoToTutorialScreen);
        creditsBtn.onClick.AddListener(OpenCredits);
        quitBtn.onClick.AddListener(QuitGame);
        settingsBtnBack.onClick.AddListener(CloseSettings);
        creditsBtnBack.onClick.AddListener(CloseCredits);
        AddHoverSound(playBtn);
        AddHoverSound(settingsBtn);
        AddHoverSound(creditsBtn);
        AddHoverSound(quitBtn);
        AddHoverSound(settingsBtnBack);
        AddHoverSound(creditsBtnBack);
    }
    private void OnDestroy()
    {
        playBtn.onClick.RemoveAllListeners();
        settingsBtn.onClick.RemoveAllListeners();
        tutorialBtn.onClick.RemoveAllListeners();
        quitBtn.onClick.RemoveAllListeners();
        settingsBtnBack.onClick.RemoveAllListeners();
        creditsBtnBack.onClick.RemoveAllListeners();
    }
    private void PlayGame()
    {
        SceneManager.LoadScene("MainScene");
    }
    private void OpenSettings() 
    {
        settingsCanvas.SetActive(true);
        mainCanvas.SetActive(false);
    }

    private void GoToTutorialScreen() => SceneManager.LoadScene("Tutorial");

    private void OpenCredits()
    {
        creditsCanvas.SetActive(true);
        mainCanvas.SetActive(false);
    }
    private void QuitGame()
    {
        Application.Quit();
    }
    private void CloseSettings()
    {
        AudioManager.Instance.PlayUI(buttonClickSfx);
        settingsCanvas.SetActive(false);
        mainCanvas.SetActive(true);
    }
    private void CloseCredits()
    {
        AudioManager.Instance.PlayUI(buttonClickSfx);
        creditsCanvas.SetActive(false);
        mainCanvas.SetActive(true);
    }
    private void AddHoverSound(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((eventData) =>
        {
            AudioManager.Instance?.PlayUI(buttonHoverSfx);
        });

        trigger.triggers.Add(entry);
    }
}