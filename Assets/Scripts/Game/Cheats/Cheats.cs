using UnityEngine;

public class Cheats : MonoBehaviour
{
    [SerializeField] private KeyCode _keyWin;
    [SerializeField] private KeyCode _keyLose;
    [SerializeField] private KeyCode _keyMoney;
    [SerializeField] private KeyCode _keyAccept;
    [SerializeField] private KeyCode _keyDeny;

    private void Update()
    {
        if (!RunManager.Instance) return;

        if (Input.GetKeyDown(_keyWin))
            WinPressed();
        if (Input.GetKeyDown(_keyLose))
            LosePressed();
        if (Input.GetKeyDown(_keyMoney))
            MoneyPressed();
        if (Input.GetKeyDown(_keyAccept))
            AcceptAll();
        if (Input.GetKeyDown(_keyDeny))
            DenyAll();
    }

    private void WinPressed() => RunManager.Instance.RoundManager.CheatCallWin();

    private void LosePressed() =>  RunManager.Instance.RoundManager.CheatCallLose();

    private void MoneyPressed() => RunManager.Instance.MoneySystem.AddMoney(99999);

    private void AcceptAll() { }

    private void DenyAll() { }
}