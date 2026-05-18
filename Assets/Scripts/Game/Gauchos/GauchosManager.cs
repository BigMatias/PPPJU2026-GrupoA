using System.Collections.Generic;
using UnityEngine;

/* ----- FACU -----
 * Este script es el que se encarga de callear a los gauchos activos, que estarían en la lista _activeGauchos}
 * Se usan 2 foreach, el primero recorre la lista
 * El 2do recorre la lista de efectos del propio gaucho
 * Le agregué unos métodos de get por si necesitamos usarlos
 * Además, le puse el SetNewGaucho para poder agregar los que se compren a la lista, sino ninguno andaría jeje
 */

public class GauchosManager : MonoBehaviour
{
    private List<GauchoInstance> _activeGauchos = new List<GauchoInstance>();

    public void SetNewGaucho(GauchoInstance instance)
    {
        _activeGauchos.Add(instance);
    }

    public GauchoInstance GetGaucho(GauchoInstance instance)
    {
        foreach (GauchoInstance item in _activeGauchos)
            if (instance == item)
                return item;

        return null;
    }

    public List<GauchoInstance> GetGaucho()
    {
        return _activeGauchos;
    }

    public void Trigger(GameEvents gameEvents, GauchoContext context)
    {
        foreach (GauchoInstance gaucho in _activeGauchos)
            foreach (GauchoEffectSO effect in gaucho.data.effects)
                effect.Execute(context, gaucho);
    }
}