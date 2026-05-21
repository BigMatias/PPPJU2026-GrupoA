using UnityEngine;

public class CardView : MonoBehaviour
{
    [SerializeField] private Sprite flippedCardImage;

    public Card card;
    private SpriteRenderer sprite;
    private SpriteRenderer outlineSR;
    private bool isFlipped = false;

    private Color yellowColor = Color.yellow;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        CreateOutline();
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

    public void SetSelected(bool value)
    {
        transform.localScale = value ? Vector3.one * 1.2f : Vector3.one;
        SetOutline(false);
    }

    private void OnMouseEnter()
    {
        // If the card is not the player's, do not show hover
        if (gameObject.layer != (int)Layers.Player) return;

        SetOutline(true);
    }

    private void OnMouseExit()
    {
        SetOutline(false);
    }

    private void SetOutline(bool visible)
    {
        if (outlineSR != null)
            outlineSR.gameObject.SetActive(visible);
    }
}