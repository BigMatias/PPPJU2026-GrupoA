using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIConsole : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI consoleText;
    [SerializeField] private int maxLines = 8;

    private Queue<string> _lines = new Queue<string>();

    public void Write(string message, ConsoleOwner owner)
    {
        string color = owner switch
        {
            ConsoleOwner.Player => "#4FC3F7",
            ConsoleOwner.Enemy  => "#EF9A9A",
            ConsoleOwner.System => "#FFFFFF",
            _ => "#FFFFFF"
        };

        string line = $"<color={color}>{message}</color>";

        _lines.Enqueue(line);
        if (_lines.Count > maxLines)
            _lines.Dequeue();

        consoleText.text = string.Join("\n", _lines);
    }
}