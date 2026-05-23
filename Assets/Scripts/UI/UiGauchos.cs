using TMPro;
using UnityEngine;

public class UiGauchos : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] _textGauchos;
    private int _gauchos = 0;

    private void OnEnable()
    {
        RunManager.Instance.Gauchos.OnAddGaucho += AddGaucho;
    }

    private void OnDisable()
    {
        RunManager.Instance.Gauchos.OnAddGaucho -= AddGaucho;
    }

    private void AddGaucho(string name)
    {
        _textGauchos[_gauchos].text = name;
        _gauchos++;
    }
}