using UnityEngine;

[CreateAssetMenu(fileName = "ExtraMult", menuName = "Gauchos/Effects/ExtraMult")]
public class EffectExtraMult : GauchoEffectSO // 6
{
    [SerializeField] private int _bonus;

    public override void Execute(GauchoContext context, GauchoInstance owner)
    {
        context.mult += _bonus;
        Debug.Log("Added mult");
    }
}
