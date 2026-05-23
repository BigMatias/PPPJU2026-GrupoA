using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
[RequireComponent(typeof(Button))]
public class ButtonSfx : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private AudioClip _clickSound;
    [SerializeField] private AudioClip _hoverSound;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_button.interactable) return;
        AudioManager.Instance.PlayUI(_hoverSound);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_button.interactable) return;
        AudioManager.Instance.PlayUI(_clickSound);
    }
}