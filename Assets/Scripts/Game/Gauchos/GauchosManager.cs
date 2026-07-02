using System;
using System.Collections.Generic;
using UnityEngine;

/* ----- FACU -----
 * Este script es el que se encarga de callear a los gauchos activos, que estar�an en la lista _activeGauchos}
 * Se usan 2 foreach, el primero recorre la lista
 * El 2do recorre la lista de efectos del propio gaucho
 * Le agregu� unos m�todos de get por si necesitamos usarlos
 * Adem�s, le puse el SetNewGaucho para poder agregar los que se compren a la lista, sino ninguno andar�a jeje
 */

public class GauchosManager : MonoBehaviour
{
    public event Action<Sprite> OnAddGaucho;

    private List<GauchoInstance> _activeGauchosList = new();
    public GauchoContext Context { get; private set; }

    private void Awake()
    {
        Context = new GauchoContext();
    }

    public void AddGauchoToRun(GauchoDataSO gauchoSO)
    {
        GauchoInstance newGaucho = new(gauchoSO);
        _activeGauchosList.Add(newGaucho);
        OnAddGaucho?.Invoke(newGaucho.data.sprite);
        Debug.Log("Se agreg� " + newGaucho.data.name + " a los gauchos activos");
    }

    public GauchoInstance GetActiveGaucho(GauchoInstance instance)
    {
        foreach (GauchoInstance item in _activeGauchosList)
            if (instance == item)
                return item;

        return null;
    }

    public List<GauchoInstance> GetActiveGauchosList() => _activeGauchosList;

    public void SetContext(int? points = null, float? mult = null, Card playedCard = null, List<Card> hand = null, int? trucosWon = null, bool? wonLastRound = null)
    {
        if (points.HasValue)
            Context.points = points.Value;

        if (mult.HasValue)
            Context.mult = mult.Value;

        if (playedCard != null)
            Context.playedCard = playedCard;

        if (hand != null)
            Context.hand = hand;

        if (trucosWon.HasValue)
            Context.trucosWon = trucosWon.Value;

        if (wonLastRound.HasValue)
            Context.wonLastRound = wonLastRound.Value;
    }

    public GauchoContext GetContext() => Context;

    public void Trigger(GameEvents gameEvents)
    {
        if (gameEvents != GameEvents.CardPlayed)
            Context.playedCard = null;

        foreach (GauchoInstance gaucho in _activeGauchosList)
            if (!gaucho.activatedThisTurn)
            {
                foreach (GauchoEffectSO effect in gaucho.data.effects)
                    effect.Execute(Context, gaucho);
                gaucho.activatedThisTurn = true;
            }
    }

    public void OnRoundEnd_ResetGauchos()
    {
        foreach (GauchoInstance gaucho in _activeGauchosList)
            gaucho.activatedThisTurn = false;
    }
}