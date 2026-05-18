using UnityEngine;

[CreateAssetMenu(fileName = "RandomMult", menuName = "Gauchos/Effects/RandomMult")]
public class EffectRandomMult : GauchoEffectSO // 3
{
    [SerializeField] private int _bonus;
    [SerializeField] private float _random;

    public override void Execute(GauchoContext context, GauchoInstance owner)
    {   
        float rand = Random.value;
        if (rand > _random)
        {
            context.mult += _bonus;
            Debug.Log("Random mult effect was successfull");
        }
        else
            Debug.Log("Random was less than 0.75");
    }
}
