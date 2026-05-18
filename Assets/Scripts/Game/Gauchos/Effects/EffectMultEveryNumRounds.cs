using UnityEngine;

[CreateAssetMenu(fileName = "MultEveryNumRounds", menuName = "Gauchos/Effects/MultEveryNumRounds")]
public class EffectMultEveryNumRounds : GauchoEffectSO // 4
{
    [SerializeField] private int _roundsToAffect;
    [SerializeField] private int _bonus;

    public override void Execute(GauchoContext context, GauchoInstance owner)
    {
        owner.stacks++;
        if (owner.stacks == _roundsToAffect)
        {
            context.mult *= _bonus;
            owner.stacks = 0;
            Debug.Log("Rounds mult effect was successfull");
        }
        else
            Debug.Log("Wait " + (_roundsToAffect - owner.stacks) + " rounds for the effect...");
    }
}
