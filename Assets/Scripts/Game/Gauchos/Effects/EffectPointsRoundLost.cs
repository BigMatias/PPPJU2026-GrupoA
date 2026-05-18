using UnityEngine;

[CreateAssetMenu(fileName = "PointsRoundLost", menuName = "Gauchos/Effects/PointsRoundLost")]
public class EffectPointsRoundLost : GauchoEffectSO // 5
{
    [SerializeField] private int _bonus;

    public override void Execute(GauchoContext context, GauchoInstance owner)
    {
        if (!context.wonLastRound)
        {
            context.points += _bonus;
            Debug.Log("Last round was lost, so added points to bonus");
        }
        else
            Debug.Log("Last round was won");
    }
}