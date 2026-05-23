using UnityEngine;

[CreateAssetMenu(fileName = "SuitMult", menuName = "Gauchos/Effects/SuitMult")]
public class EffectSuitMult : GauchoEffectSO // 2
{
    [SerializeField] private Suit _targetSuit;
    [SerializeField] private int _bonus;

    public override void Execute(GauchoContext context, GauchoInstance owner)
    {
        if (context.playedCard == null || context.playedCard.cardDataSO == null) return;

        if (context.playedCard.cardDataSO.suit == _targetSuit)
        {
            context.mult += _bonus;
            Debug.Log("Suit mult effect was successful");
        }
        else
            Debug.Log("Suits weren't equal");
    }
}
