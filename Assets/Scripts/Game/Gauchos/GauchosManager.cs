using System.Collections.Generic;
using UnityEngine;

/* ----- FACU -----
 * Este script es el que se encarga de callear a los gauchos activos, que estarían en la lista _activeGauchos}
 * Se usan 2 foreach, el primero recorre la lista
 * El 2do recorre la lista de efectos del propio gaucho
 */

public class GauchosManager : MonoBehaviour
{
    private List<GauchoInstance> _activeGauchos = new List<GauchoInstance>();

    public void Trigger(GameEvents gameEvents, GauchoContext context)
    {
        foreach (GauchoInstance gaucho in _activeGauchos)
            foreach (GauchoEffectSO effect in gaucho.data.effects)
                effect.Execute(context, gaucho);
    }
}