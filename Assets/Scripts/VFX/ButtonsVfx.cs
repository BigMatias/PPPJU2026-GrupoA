using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonsVfx : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Vector3 _maxSize = new(1.35f, 1.35f, 1.35f);
    [SerializeField] private float _time = 0.5f;
    private Vector3 _initSize;

    private void Start()
    {
        _initSize = transform.localScale;
        DOTween.Init();
    }

    private void OnDestroy()
    {
        DOTween.Clear();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(_maxSize, _time);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(_initSize, _time);
    }
}