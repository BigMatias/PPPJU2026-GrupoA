using UnityEngine;

[CreateAssetMenu(fileName = "MultForFlor", menuName = "Gauchos/Effects/MultForFlor")]
public class EffectMultForFlor : GauchoEffectSO // 1
{
    [SerializeField] private int _bonus;

    public override void Execute(GauchoContext context, GauchoInstance owner)
    {
        if (context.hand[0].cardDataSO.suit == context.hand[1].cardDataSO.suit && context.hand[0].cardDataSO.suit == context.hand[2].cardDataSO.suit)
        {
            context.mult += _bonus;
            Debug.Log("Suit mult for FLOR effect was successful");
        }
        else
            Debug.Log("Suits weren't equal");
    }
}
