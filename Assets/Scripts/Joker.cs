using UnityEngine;

public class Joker
{
    private Sprite _sprite;
    private Rarity _rarity;
    private int _pointsEffect;
    private int _multEffect;

    public Joker(JokerDataSO data)
    {
        _sprite = data.sprite;
        _rarity = data.rarity;
        _pointsEffect = data.pointsEffect;
        _multEffect = data.multEffect;
    }

    ~Joker() { }

    public virtual void Init(SpriteRenderer renderer)
    {
        renderer.sprite = _sprite;
    }
}