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
    private List<GauchoInstance> _activeGauchosList = new List<GauchoInstance>();

    public void AddGauchoToRun(GauchoDataSO gauchoSO)
    {
        GauchoInstance newGaucho = new(gauchoSO);
        _activeGauchosList.Add(newGaucho);
        Debug.Log("Se agregó " + newGaucho.data.name + " a los gauchos activos");
    }

    public GauchoInstance GetActiveGaucho(GauchoInstance instance)
    {
        foreach (GauchoInstance item in _activeGauchosList)
            if (instance == item)
                return item;

        return null;
    }

    public List<GauchoInstance> GetActiveGauchosList()
    {
        return _activeGauchosList;
    }

    public void Trigger(GameEvents gameEvents, GauchoContext context)
    {
        foreach (GauchoInstance gaucho in _activeGauchosList)
            foreach (GauchoEffectSO effect in gaucho.data.effects)
                effect.Execute(context, gaucho);
    }
}