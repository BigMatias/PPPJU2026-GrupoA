using UnityEngine;

public class CardView : MonoBehaviour
{
    [SerializeField] private Sprite flippedCardImage;
    private Card card;
    private SpriteRenderer sprite;

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
        sprite.sprite = flippedCardImage;
    }
}