using UnityEngine;

public class CardView : MonoBehaviour
{
    [SerializeField] private Sprite flippedCardImage;
    private Card card;
    private SpriteRenderer sprite;
    private bool isFlipped = false;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();    
    }

    public void Setup(Card card)
    {
        this.card = card;
        sprite.sprite = card.cardDataSO.artwork;
    }

    public void Flip (Card card)
    {
        this.card = card;
        if (!isFlipped)
        {
            sprite.sprite = flippedCardImage;
        }
        else
        {
            sprite.sprite = card.cardDataSO.artwork;
        }
    }
    public void SetSelected(bool value)
    {
        transform.localScale = value ? Vector3.one * 1.2f : Vector3.one;
    }

    public void PlayCard()
    {

    }
}