using UnityEngine;

public class Cheats : MonoBehaviour
{
    [SerializeField] private KeyCode _keyWin;
    [SerializeField] private KeyCode _keyLose;
    [SerializeField] private KeyCode _keyMoney;

    private void Update()
    {
        if (!RunManager.Instance) return;

        if (Input.GetKeyDown(_keyWin))
            WinPressed();
        if (Input.GetKeyDown(_keyLose))
            LosePressed();
        if (Input.GetKeyDown(_keyMoney))
            MoneyPressed();
    }

    private void WinPressed() => RunManager.Instance.RoundManager.CheatCallWin();

    private void LosePressed() =>  RunManager.Instance.RoundManager.CheatCallLose();

    private void MoneyPressed() => RunManager.Instance.MoneySystem.AddMoney(99999);
}