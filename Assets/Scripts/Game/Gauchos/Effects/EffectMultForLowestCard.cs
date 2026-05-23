using UnityEngine;

[CreateAssetMenu(fileName = "MultForLowestCard", menuName = "Gauchos/Effects/MultForLowestCard")]
public class EffectMultForLowestCard : GauchoEffectSO // 7
{
    [SerializeField] private int _bonus;

    public override void Execute(GauchoContext context, GauchoInstance owner)
    {
        int lowestCard = 999999999;

        if (context.hand != null)
            foreach (Card card in context.hand)
                if (card != null && card.cardDataSO.value < lowestCard)
                    lowestCard = card.cardDataSO.value;

        if (context.playedCard.cardDataSO.value == lowestCard)
        {
            context.mult *= _bonus;
            Debug.Log("Mult for lowest card effect was successful");
        }
        else
            Debug.Log("Card wasn't the lowest");
    }
}
