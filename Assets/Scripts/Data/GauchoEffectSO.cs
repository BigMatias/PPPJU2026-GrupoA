using UnityEngine;

/* ----- FACU -----
 * Como una FSM, pero solo con Execute() por ahora
 * Es para que todos los gauchos compartan la misma clase, casi como si fuese una interfaz :p
 */

public abstract class GauchoEffectSO : ScriptableObject
{
    public abstract void Execute(GauchoContext context, GauchoInstance owner);
}