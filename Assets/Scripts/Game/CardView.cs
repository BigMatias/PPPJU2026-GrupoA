using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class CardView : MonoBehaviour
{
    [SerializeField] private Sprite flippedCardImage;
    [SerializeField] private Vector3 _maxSize = new(1.3f, 1.3f, 1.3f);
    [SerializeField] private float _time = 0.5f;

    public Card card;
    private SpriteRenderer sprite;
    private SpriteRenderer outlineSR;
    private bool isFlipped = false;

    private Color yellowColor = Color.yellow;

    private Vector3 _initSize;

    private bool _canBeSelected = false;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        CreateOutline();
    }

    private void Start()
    {
        DOTween.Init();
        _initSize = transform.localScale;
        _maxSize += transform.localScale;
    }

    private void OnDestroy()
    {
        DOTween.Clear();
    }

    // Children game object with SpriteRenderer for outline effect
    private void CreateOutline()
    {
        GameObject outlineGO = new GameObject("Outline");
        outlineGO.transform.SetParent(transform);
        outlineGO.transform.localPosition = Vector3.zero;
        outlineGO.transform.localScale = Vector3.one * 1.04f;

        outlineSR = outlineGO.AddComponent<SpriteRenderer>();
        outlineSR.color = yellowColor;
        outlineSR.sortingLayerName = sprite.sortingLayerName;
        outlineSR.sortingOrder = sprite.sortingOrder - 1; // Behind card

        outlineGO.SetActive(false);
    }

    public void Setup(Card card)
    {
        this.card = card;
        sprite.sprite = card.cardDataSO.artwork;
        outlineSR.sprite = card.cardDataSO.artwork; // Same sprite, yellow tint
    }

    public void Flip(Card card)
    {
        this.card = card;
        if (!isFlipped)
        {
            sprite.sprite = flippedCardImage;
            outlineSR.sprite = flippedCardImage;
            isFlipped = true;
        }
        else
        {
            sprite.sprite = card.cardDataSO.artwork;
            outlineSR.sprite = card.cardDataSO.artwork;
            isFlipped = false;
        }
    }

    public void SetSelected()
    {
        _canBeSelected = false;
        DOTween.Clear();
        transform.localScale = Vector3.one;
        SetOutline(false);
    }

    private void OnMouseEnter()
    {
        // If the card is not the player's, do not show hover
        if (gameObject.layer != (int)Layers.Player) return;

        SetOutline(true);
        OnCardPointed_ChangeSize(true);
    }

    private void OnMouseExit()
    {
        SetOutline(false);
        OnCardPointed_ChangeSize(false);
    }

    private void SetOutline(bool visible)
    {
        if (outlineSR != null)
            outlineSR.gameObject.SetActive(visible);
    }

    private void OnCardPointed_ChangeSize(bool isPointerEnter)
    {
        if (!_canBeSelected) return;
        Vector3 goal = isPointerEnter ? _maxSize : _initSize;
        transform.DOScale(goal, _time);
    }

    public void SetForPlayer() => _canBeSelected = true;
}