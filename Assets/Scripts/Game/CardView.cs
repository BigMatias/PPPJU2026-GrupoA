using UnityEngine;
using DG.Tweening;

public class CardView : MonoBehaviour
{
    [SerializeField] private Sprite flippedCardImage;
    [SerializeField] private float _maxIncreaseInSize = 1.3f;
    [SerializeField] private float _time = 0.5f;
    [SerializeField] private Vector3 _tableSize = new(1f, 1f, 1f);
    [SerializeField] private ParticleSystem _particles;

    public Card card;
    private SpriteRenderer _renderer;
    private SpriteRenderer outlineSR;
    private bool isFlipped = false;

    private Color yellowColor = Color.yellow;

    private Vector3 _initPos;
    private Vector3 _initSize;
    private Vector3 _maxSize;

    private bool _canBeSelected = false;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        CreateOutline();
    }

    private void Start()
    {
        DOTween.Init();
        _initPos = transform.localPosition;
        _initSize = transform.localScale;
        _maxSize = transform.localScale * _maxIncreaseInSize;
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);
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
        outlineSR.sortingLayerName = _renderer.sortingLayerName;
        outlineSR.sortingOrder = _renderer.sortingOrder - 1; // Behind card

        outlineGO.SetActive(false);
    }

    public void Setup(Card card)
    {
        this.card = card;
        _renderer.sprite = card.cardDataSO.artwork;
        outlineSR.sprite = card.cardDataSO.artwork; // Same sprite, yellow tint
    }

    public void Flip(Card card)
    {
        this.card = card;
        if (!isFlipped)
        {
            _renderer.sprite = flippedCardImage;
            outlineSR.sprite = flippedCardImage;
            isFlipped = true;
        }
        else
        {
            _renderer.sprite = card.cardDataSO.artwork;
            outlineSR.sprite = card.cardDataSO.artwork;
            isFlipped = false;
        }
    }

    public void SetSelected()
    {
        _canBeSelected = false;
        transform.DOKill();
        transform.DOScale(_tableSize, 0.15f);
        SetOutline(false);
        SetOffParticles();
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
        transform.DOKill();
        Vector3 goal = isPointerEnter ? _maxSize : _initSize;
        transform.DOScale(goal, _time);
    }

    public void SetForPlayer() => _canBeSelected = true;

    private void SetOffParticles() => _particles.Play();

    public void SetCardToWinner()
    {
        transform.DOKill();
        transform.localScale = _tableSize;
        transform.DOPunchScale(_tableSize * 1.05f, 1f, 3, .35f);
    }

    public void CardDenided()
    {
        transform.DOKill();
        transform.localPosition = _initPos;

        float duration = 0.25f;

        float rand = Random.value;
        Vector3 moveHor;
        if (rand < 0.5f)
            moveHor = new(0.2f, 0f, 0f);
        else
            moveHor = new(-0.2f, 0f, 0f);

        transform.DOPunchPosition(moveHor, duration);
    }
}