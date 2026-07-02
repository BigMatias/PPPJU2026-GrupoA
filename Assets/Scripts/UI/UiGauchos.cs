using UnityEngine;
using UnityEngine.UI;

public class UiGauchos : MonoBehaviour
{
    [SerializeField] private Image[] _gauchoImages;
    private int _gauchos = 0;

    private void Start()
    {
        foreach (Image renderer in _gauchoImages)
        {
            renderer.color = new(255f, 255f, 255f, 0f);
        }

        RunManager.Instance.Gauchos.OnAddGaucho += AddGaucho;
    }

    private void OnDisable()
    {
        RunManager.Instance.Gauchos.OnAddGaucho -= AddGaucho;
    }

    private void AddGaucho(Sprite sprite)
    {
        _gauchoImages[_gauchos].color = new(255f, 255f, 255f, 1f);
        _gauchoImages[_gauchos].sprite = sprite;
        _gauchos++;
    }
}